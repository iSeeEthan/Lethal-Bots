using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LethalBots.Managers
{
    /// <summary>
    /// Manager in charge of finding, caching, and synchronize clients with the terminal!
    /// </summary>
    class TerminalManager : NetworkBehaviour
    {
        public static TerminalManager Instance { get; internal set; } = null!;

        private Terminal? terminalScript = null;

        private void Awake()
        {
            Instance = this;
        }

        public Terminal GetTerminal()
        {
            if (terminalScript == null)
            {
                terminalScript = GameObject.Find("TerminalScript").GetComponent<Terminal>();
            }
            return terminalScript;
        }
    }
}
