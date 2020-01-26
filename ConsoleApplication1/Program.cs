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
        public const double distanceForward = 10;

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
            IMyCubeBlock reference = P.gridTerminalSystem.GetBlockWithName("MainScript") as IMyCubeBlock;

            Vector3D fwd = reference.WorldMatrix.Forward - reference.WorldMatrix.Backward;
            fwd.Normalize(); //(Need to normalize because the above matrices are scaled by grid size)


            Vector3D dirActual = fwd;
            Screen.AddText("Direction", dirActual.ToString());
            return  Util.Vector3DToVector2D(dirActual);
        }
    }

    class Program
    {
        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            // Inicialitzem P
            P.Initialize(GridTerminalSystem);

            // Inicialitzem Screen
            Screen.Initialize();

            // Estat incial
            Vector2D posInicial = P.GetPosition();
            Vector2D dirInicial = P.GetDirection();
            double distance = P.distanceForward;
            P.state = new Forward(new StateDTO(posInicial, dirInicial, distance, posInicial, dirInicial));
        }

        static void Main(string[] args)
        {
            P.wheelGroup.PropulsionOverride(P.propulsionOverride);
            P.wheelGroup.SteeringOverride(P.steeringOverride);
            P.state = P.state.NextState();
            
            Screen.Show();
        }
    }
}

