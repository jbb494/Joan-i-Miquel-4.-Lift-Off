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

namespace ConsoleApplication1
{
    struct StateDTO
    {
        public Vector2D posInicial;
        public Vector2D dirInicial;
        public double distance;
        public Vector2D posActual;
        public Vector2D dirActual;

        public StateDTO(Vector2D posInicial, Vector2D dirInicial, double distance, Vector2D posActual, Vector2D dirActual)
        {
            this.posInicial = posInicial;
            this.dirInicial = dirInicial;
            this.distance = distance;
            this.posActual = posActual;
            this.dirActual = dirActual;
        }
    }

    abstract class State
    {
        private StateDTO stateProperties;

        protected StateDTO sp
        {
            get { return stateProperties; }
            set { stateProperties = value; }
        }

        public State(StateDTO dto)
        {
            this.stateProperties = dto;
        }
        public abstract State NextState();
        protected void updateDTO()
        {
            Vector2D posActual = P.GetPosition();
            Vector2D dirActual = P.GetDirection();
            this.stateProperties = new StateDTO(this.sp.posInicial, this.sp.dirInicial, this.sp.distance, posActual, dirActual);
        }
    }
    class Forward : State
    {
        public Forward(StateDTO dto) : base(dto)
        {
            Screen.AddText("State", "Forward");
            P.wheelGroup.SteeringOverride(0);
            P.wheelGroup.PropulsionOverride(P.propulsionOverride);
        }

        public override State NextState()
        {
            this.updateDTO();
            double distDone = Vector2D.Distance(this.sp.posActual, this.sp.posInicial);
            if (distDone < this.sp.distance)
            {
                return new Forward(this.sp);
            }
            else
            {
                return new Turn(new StateDTO(this.sp.posActual, this.sp.dirActual, this.sp.distance, this.sp.posActual, this.sp.dirActual));
            }
        }
    }

    class Turn : State
    {
        public Turn(StateDTO dto) : base(dto)
        {
            Screen.AddText("State", "Turn");
            P.wheelGroup.PropulsionOverride(P.propulsionOverride);
            P.wheelGroup.SteeringOverride(P.steeringOverride);
        }

        public override State NextState()
        {
            this.updateDTO();

            // dirActual i dirInicialPerpendicular
            Vector2D dirInicialP = new Vector2D(this.sp.dirInicial.Y, -1 * this.sp.dirInicial.X);
            double angleBetween = Util.AngleBetween(dirInicialP, this.sp.dirActual);
            Screen.AddText("AngleBetween" , angleBetween.ToString());

            if (angleBetween > 5)
            {
                return new Turn(this.sp);
            }
            else
            {
                return new Forward2(new StateDTO(this.sp.posActual, this.sp.dirActual, this.sp.distance, this.sp.posActual, this.sp.posActual));
            }
        }
    }

    class Forward2 : State
    {
        public Forward2(StateDTO dto) : base(dto)
        {
            Screen.AddText("State", "Forward2");
            P.wheelGroup.PropulsionOverride(P.propulsionOverride);
            P.wheelGroup.SteeringOverride(0);
        }

        public override State NextState()
        {
            this.updateDTO();
            double distDone = Vector2D.Distance(this.sp.posActual, this.sp.posInicial);
            if (distDone < this.sp.distance)
            {
                return new Forward2(this.sp);
            }
            else
            {
                return new Stop(new StateDTO(this.sp.posActual, this.sp.dirInicial, this.sp.distance, this.sp.posActual, this.sp.dirActual));
            }
        }
    }

    class Stop : State
    {
        public Stop(StateDTO dto) : base(dto)
        {
            Screen.AddText("State", "Stop");
            P.wheelGroup.PropulsionOverride(0);
            P.wheelGroup.SteeringOverride(0);
        }
        public override State NextState()
        {
            return new Stop(this.sp);
        }
    }
}
