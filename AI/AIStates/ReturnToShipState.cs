using DunGen;
using GameNetcodeStuff;
using LethalBots.Constants;
using LethalBots.Enums;
using LethalBots.Managers;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace LethalBots.AI.AIStates
{
    /// <summary>
    /// State where the bots choose to return to the ship
    /// The bot will attempt to pick the safest route back to the ship!
    /// </summary>
    public class ReturnToShipState : AIState
    {
        private Vector3 targetShipPos; // The point on the ship we want to go to
        private Vector3 targetEntrancePos; // The position we want to path to reach the entrance!
        private bool attemptedToUseTZP = false;
        private float findEntranceTimer;

        public ReturnToShipState(AIState oldState) : base(oldState)
        {
            CurrentState = EnumAIStates.ReturnToShip;

            // Lets pick a random node on the ship to go to
            Transform[] ourShip = StartOfRound.Instance.insideShipPositions;
            this.targetShipPos = RoundManager.Instance.GetNavMeshPosition(ourShip[Random.Range(0, ourShip.Length - 1)].position, default, 2.7f);
        }

        public override void DoAI()
        {
            // Check for enemies
            EnemyAI? enemyAI = ai.CheckLOSForEnemy(Const.LETHAL_BOT_FOV, Const.LETHAL_BOT_ENTITIES_RANGE, (int)Const.DISTANCE_CLOSE_ENOUGH_HOR);
            if (enemyAI != null)
            {
                ai.State = new PanikState(this, enemyAI);
                return;
            }

            // Check for object to grab
            if (ai.HasSpaceInInventory())
            {
                GrabbableObject? grabbableObject = ai.LookingForObjectToGrab();
                if (grabbableObject != null)
                {
                    ai.State = new FetchingObjectState(this, grabbableObject);
                    return;
                }
            }

            // If we are inside, we need to move outside first!
            if (!ai.isOutside)
            {
                // If we don't have an entrance selected we should pick one now!
                if (targetEntrance == null 
                    || findEntranceTimer > Const.RETURN_UPDATE_ENTRANCE)
                {
                    findEntranceTimer = 0f;
                    targetEntrance = FindClosestEntrance(this.targetShipPos);
                    if (targetEntrance == null)
                    {
                        // Find a door that might help us escape!!!!
                        DoorLock? lockedDoor = ai.UnlockDoorIfNeeded(200f, false);
                        if (lockedDoor != null)
                        {
                            ai.State = new UseKeyOnLockedDoorState(this, lockedDoor);
                            return;
                        }

                        // If we fail to find an entrance or a locked door, we should look for players instead!
                        ai.State = new LostInFacilityState(this);
                        return;
                    }
                }
                else
                {
                    findEntranceTimer += ai.AIIntervalTime;
                }

                // Alright lets go back outside!
                // If we need to go up an elevator we should do so!
                if (ai.ElevatorScript != null && !ai.IsInElevatorStartRoom && !ai.IsValidPathToTarget(targetEntrance.entrancePoint.position, false))
                {
                    // Use elevator returns a bool if the can successfully use the elevator
                    if (!ai.UseElevator(true))
                    {
                        // If we can't path to the elevator, there might be a locked door in our way!
                        // The chances of this are VERY low normally, but it may happen if we used an inverse teleporter!
                        DoorLock? lockedDoor = ai.UnlockDoorIfNeeded(200f, false);
                        if (lockedDoor != null)
                        {
                            ai.State = new UseKeyOnLockedDoorState(this, lockedDoor);
                            return;
                        }

                        // If we fail to find an entrance or a locked door, we should look for players instead!
                        ai.State = new LostInFacilityState(this);
                        return;
                    }
                    else
                    {
                        // NOTE: ai.destination is set by ai.UseElevator internally!
                        targetEntrancePos = ai.destination;
                    }
                }
                else
                {
                    targetEntrancePos = targetEntrance.entrancePoint.position;

                    // If we can't path to the exit, there might be a locked door in our way!
                    // The chances of this are VERY low normally, but it may happen if we used an inverse teleporter!
                    if (!ai.IsValidPathToTarget(targetEntrance.entrancePoint.position, false))
                    {
                        DoorLock? lockedDoor = ai.UnlockDoorIfNeeded(200f, false);
                        if (lockedDoor != null)
                        {
                            ai.State = new UseKeyOnLockedDoorState(this, lockedDoor);
                            return;
                        }
                    }
                }

                // Find a safe path to the entrance
                StartSafePathCoroutine();

                // If we are close enough, we should use the entrance to leave
                float entranceDistSqr = (this.targetEntrance.entrancePoint.position - npcController.Npc.transform.position).sqrMagnitude;
                if (entranceDistSqr >= Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION * Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION)
                {
                    float sqrMagDistanceToSafePos = (this.safePathPos - npcController.Npc.transform.position).sqrMagnitude;
                    if (sqrMagDistanceToSafePos >= Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION * Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION)
                    {
                        // Alright lets go outside!
                        ai.SetDestinationToPositionLethalBotAI(this.safePathPos);

                        // Sprint if far enough from the ship
                        if (!npcController.WaitForFullStamina && sqrMagDistanceToSafePos > Const.DISTANCE_START_RUNNING * Const.DISTANCE_START_RUNNING) // NEEDTOVALIDATE: Should we use the distance to the ship or the safe position?
                        {
                            npcController.OrderToSprint();
                        }
                        else
                        {
                            npcController.OrderToStopSprint();
                        }

                        ai.OrderMoveToDestination();
                    }
                    else
                    {
                        // Wait here until its safe to move to the entrance
                        ai.StopMoving();
                        npcController.OrderToStopSprint();
                    }

                }
                else
                {
                    // Check for teleport entrance
                    if (!ai.AreHandsFree() && ai.HeldItem is CaveDwellerPhysicsProp)
                    {
                        // We must drop the maneater baby before we use the entrance!
                        ai.DropItem();
                        return;
                    }
                    else if (Time.timeSinceLevelLoad - ai.TimeSinceTeleporting > Const.WAIT_TIME_TO_TELEPORT)
                    {
                        Vector3? entranceTeleportPos = ai.GetTeleportPosOfEntrance(targetEntrance);
                        if (entranceTeleportPos.HasValue)
                        {
                            Plugin.LogDebug($"======== TeleportLethalBotAndSync {ai.NpcController.Npc.playerUsername} !!!!!!!!!!!!!!! ");
                            ai.StopMoving();
                            ai.SyncTeleportLethalBot(entranceTeleportPos.Value, !this.targetEntrance?.isEntranceToBuilding ?? !ai.isOutside, this.targetEntrance);
                        }
                        else
                        {
                            // HOW DID THIS HAPPEN!
                            ai.State = new LostInFacilityState(this);
                            return;
                        }
                    }
                }
            }
            else
            {
                // If we have TZP we should attempt to use to speed up our return trip if needed!
                if (!attemptedToUseTZP)
                {
                    attemptedToUseTZP = true;
                    if (ai.HasGrabbableObjectInInventory(typeof(TetraChemicalItem), out int _))
                    {
                        ai.State = new UseTZPInhalantState(this, GetDesiredDrunknessAmount());
                        return;
                    }
                }

                // Keep moving towards the ship!
                float sqrMagDistanceToShip = (this.targetShipPos - npcController.Npc.transform.position).sqrMagnitude;
                if (sqrMagDistanceToShip >= Const.DISTANCE_TO_CHILL_POINT * Const.DISTANCE_TO_CHILL_POINT)
                {
                    // Find a safe path to the ship
                    StartSafePathCoroutine();

                    float sqrMagDistanceToSafePos = (this.safePathPos - npcController.Npc.transform.position).sqrMagnitude;
                    if (sqrMagDistanceToSafePos >= Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION * Const.DISTANCE_CLOSE_ENOUGH_TO_DESTINATION)
                    {
                        // Move to the ship!
                        ai.SetDestinationToPositionLethalBotAI(this.safePathPos);

                        // Sprint if far enough from the ship
                        if (!npcController.WaitForFullStamina && sqrMagDistanceToSafePos > Const.DISTANCE_START_RUNNING * Const.DISTANCE_START_RUNNING) // NEEDTOVALIDATE: Should we use the distance to the ship or the safe position?
                        {
                            npcController.OrderToSprint();
                        }
                        else
                        {
                            npcController.OrderToStopSprint();
                        }

                        ai.OrderMoveToDestination();
                    }
                    else
                    {
                        // Wait here until its safe to move to the ship
                        ai.StopMoving();
                        npcController.OrderToStopSprint();
                    }
                }
                else
                {
                    // We are at the ship, lets wait here for a bit!
                    ai.State = new ChillAtShipState(this);
                    return;
                }
            }
        }

        /// <remarks>
        /// In this state, we do not want to only use the front entrance,
        /// </remarks>
        /// <inheritdoc cref="AIState.ShouldOnlyUseFrontEntrance"></inheritdoc>
        protected override bool ShouldOnlyUseFrontEntrance()
        {
            return false;
        }

        // Just play following voice lines for now!
        public override void TryPlayCurrentStateVoiceAudio()
        {
            // Default states, wait for cooldown and if no one is talking close
            ai.LethalBotIdentity.Voice.TryPlayVoiceAudio(new PlayVoiceParameters()
            {
                VoiceState = EnumVoicesState.FollowingPlayer,
                CanTalkIfOtherLethalBotTalk = false,
                WaitForCooldown = true,
                CutCurrentVoiceStateToTalk = false,
                CanRepeatVoiceState = true,

                ShouldSync = true,
                IsLethalBotInside = npcController.Npc.isInsideFactory,
                AllowSwearing = Plugin.Config.AllowSwearing.Value
            });
        }

        public override void OnSignalTranslatorMessageReceived(string message)
        {
            // Already heading back to ship!
            if (message == "return")
            {
                return;
            }
            base.OnSignalTranslatorMessageReceived(message);
        }

        /// <summary>
        /// Helper function that returns how much <see cref="TetraChemicalItem"/> aka TZP the bot wants to use,
        /// based on how encumbered it is (i.e., how much slower it is due to carry weight).
        /// </summary>
        /// <returns>The desired <see cref="PlayerControllerB.drunkness"/> level, clamped between 0 and 1.</returns>
        private float GetDesiredDrunknessAmount()
        {
            // Assume default base speed is 4.5f unless overridden
            float baseMoveSpeed = npcController.Npc.movementSpeed > 0f ? npcController.Npc.movementSpeed : 4.5f;

            // Current effective speed = base / weight
            float effectiveMoveSpeed = baseMoveSpeed / npcController.Npc.carryWeight;

            // Determine how much speed we've lost due to weight
            float speedLossRatio = Mathf.Clamp01(1f - (effectiveMoveSpeed / baseMoveSpeed));

            // Scale desired drunkness based on how much speed is lost
            float desiredDrunkness = Mathf.Clamp01(speedLossRatio * 1.2f); // 120% multiplier for tuning

            // Cutoff: ignore small slowdowns to avoid unnecessay TZP use
            if (desiredDrunkness < Const.DRUNKNESS_CUTOFF)
            {
                return 0f;
            }

            // Log for testing purposes!
            Plugin.LogDebug($"Bot {npcController.Npc.playerUsername}: BaseSpeed={baseMoveSpeed:F2}, EffectiveSpeed={effectiveMoveSpeed:F2}, Drunkness={desiredDrunkness:F2}");
            return desiredDrunkness;
        }

        /// <remarks>
        /// We give the ship position we want a safe path to!<br/>
        /// We return null if we are not outside!
        /// </remarks>
        /// <inheritdoc cref="AIState.GetDesiredSafePathPosition"></inheritdoc>
        protected override Vector3? GetDesiredSafePathPosition()
        {
            if (ai.isOutside)
            {
                return this.targetShipPos;
            }
            return this.targetEntrance != null ? this.targetEntrancePos : null;
        }

        /// <remarks>
        /// We ignore the intital danger check if we on the ship!<br/>
        /// Unless the ship is compromised, then we wait outside!
        /// </remarks>
        /// <inheritdoc cref="AIState.ShouldIgnoreInitialDangerCheck"></inheritdoc>
        protected override bool ShouldIgnoreInitialDangerCheck()
        {
            if (!ai.isOutside || LethalBotManager.IsShipCompromised(ai))
            {
                return base.ShouldIgnoreInitialDangerCheck();
            }
            return npcController.Npc.isInElevator || npcController.Npc.isInHangarShipRoom;
        }
    }
}
