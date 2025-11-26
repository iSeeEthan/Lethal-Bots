namespace LethalBots.Constants
{
    // TODO: Change this to help the MissionControlState with purchasing items, since this is legacy code from the original mod Lethal Internship!
    public class TerminalConst
    {
        public const int INDEX_HELP_TERMINALNODE = 13;
        public const string STRING_OTHER_HELP = ">OTHER";
        public const int INDEX_AUDIO_BOUGHT_ITEM = 0;
        public const int INDEX_AUDIO_ERROR = 1;

        public const string STRING_BUY_COMMAND = "buy";
        public const string STRING_LAND_COMMAND = "land";
        public const string STRING_STATUS_COMMAND = "status";

        public const string STRING_CONFIRM_COMMAND = "confirm";
        public const string STRING_CANCEL_COMMAND = "deny";
        public const string STRING_BACK_COMMAND = "back";

        public const string STRING_LANDING_STATUS_ALLOWED = "+ Allowed +";
        public const string STRING_LANDING_STATUS_ABORTED = "--Aborted--";
        public const string STRING_LANDING_STATUS_ABORTED_COMPANY_MOON = " (aborted on company building moon)";

        public const string STRING_INTERNSHIP_PROGRAM_HELP = ">{0}\n{1}\n\n";


        public const string TEXT_INFO_PAGE_IN_SPACE = @"Remaining interns : {0}
Unit price : ${1}

----------------------------------------
--> Interns scheduled on next moon : {2}
----------------------------------------

Interns landing on moon : {3}


Commands :
- 'land'  : allow/cancel
- 'status': list of interns
- 'buy'   : order new intern, 'buy 2' to order 2 interns, etc";

        public const string TEXT_INFO_PAGE_INTERN_TO_DROPSHIP = "Interns waiting to land : {0}";
        public const string TEXT_INFO_PAGE_ON_MOON = @"Remaining interns : {0}
Unit price : ${1}

{2}
------------------------------
--> Interns on this moon : {3}
------------------------------

Interns landing status : {4}


Commands :
- 'land'  : allow/cancel
- 'status': list of interns
- 'buy'   : order new intern, 'buy 2' to order 2 interns, etc";


        public const string TEXT_CONFIRM_CANCEL_PURCHASE_MAXIMUM = "(max)";
        public const string TEXT_CONFIRM_CANCEL_PURCHASE = @"You have requested to order interns. Amount {0}{1}

Total cost of items: ${2}


Please CONFIRM or DENY.";
        public const string TEXT_CONFIRM_CANCEL_REVIVE_INTERN = " (will be reanimated)";
        public const string TEXT_CONFIRM_CANCEL_SPECIFIC_PURCHASE = @"You have select {0} for the next drop.{1}

Total cost : ${2}


Please CONFIRM or DENY.";

        public const string TEXT_ERROR_DEFAULT = @"An error occured in the internship program.";
        public const string TEXT_ERROR_NOT_ENOUGH_CREDITS = @"You do not have enough credits to order an intern.";
        public const string TEXT_NO_MORE_INTERNS_PURCHASABLE = @"Sorry too much interns at once. Try again later.";
        public const string TEXT_ERROR_SHIP_LEAVING = @"You can not buy when the ship is leaving the moon.";
        public const string TEXT_ERROR_INTERN_DEAD = @"Sorry, this intern is dead, you can revive an intern only in orbit, try with another one.";
        public const string TEXT_ERROR_INTERN_ALREADY_SELECTED = @"Sorry, this intern is already selected for the next moon, try with another one.";

        public const string TEXT_STATUS = @"Interns status : 

{0}";
    }
}
