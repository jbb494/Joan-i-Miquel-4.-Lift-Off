using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRageMath;
using VRage.Game.ModAPI.Ingame;

namespace ConsoleApplication1
{
    class P {
        // CONSTANTS: 
        public const double propulsionOverride = 0.06;
        public const double steeringOverride = 0.8;

        // Atributs statics publics
        public static State state;
        public static IMyGridTerminalSystem gridTerminalSystem;
        public static WheelGroup wheelGroup;
        
        public static void Initialize(IMyGridTerminalSystem GridTerminalSystem) {
            // Access to GridTerminalSystem
            P.gridTerminalSystem = GridTerminalSystem;

            // Wheel Group
            List<IMyMotorSuspension> wheels = new List<IMyMotorSuspension>();
            P.gridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(wheels);
            P.wheelGroup = new WheelGroup(wheels);
        }

        public static Vector2D GetPosition() {
            Vector3D posActual = P.gridTerminalSystem.GetBlockWithName("MainScript").GetPosition();
            Screen.AddText("Position: ", posActual.ToString());
            return Util.Vector3DToVector2D(posActual);
        }

        public static Vector2D GetDirection() {
            IMyEntity compu = P.gridTerminalSystem.GetBlockWithName("MainScript");
            IMyEntity antenna = P.gridTerminalSystem.GetBlockWithName("AntennaBack");
            Vector3D dirBackward = antenna.GetPosition();
            Vector3D dirForward = compu.GetPosition();
            Vector3D dirActual = dirBackward - dirForward;
            Screen.AddText("Direction", dirActual.ToString());
            return  Util.Vector3DToVector2D(dirActual);
        }
    }

    static class Screen {
        private static List<IMyTextPanel> screens;
        private static string text;
        public static void Initialize() {
            Screen.screens = new List<IMyTextPanel>();
            P.gridTerminalSystem.GetBlocksOfType<IMyTextPanel>(screens);
            Screen.Clean();
        }
        public static void AddText(string DebugName, string textArg) {
            Screen.text = text + DebugName + ": " + textArg + "\n";
        }

        public static void Clean() {
            Screen.text = "";
        }

        public static void Show() {
            Screen.screens.ForEach(delegate (IMyTextPanel screen) {
                screen.WriteText(Screen.text);
            });
        }
    }

    class Program
    {
        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            // Inicialitzem P
            P.Initialize(GridTerminalSystem);

            // Inicialitzem Screen
            Screen.Initialize();

            // Estat incial
            Vector2D posInicial = P.GetPosition();
            Vector2D dirInicial = P.GetDirection();
            double distance = 30;
            P.state = new Forward(new StateDTO(posInicial, dirInicial, distance, posInicial, dirInicial));
        }

        static void Main(string[] args)
        {
            P.state = P.state.NextState();
            
            Screen.Show();
        }
    }
}

