﻿using System;
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
    public class WheelGroup
    {
        List<IMyMotorSuspension> wheelGroup;
        public WheelGroup(List<IMyMotorSuspension> wheelGroupArg)
        {
            this.wheelGroup = wheelGroupArg;
        }
        public void PropulsionOverride(double i)
        {
            this.wheelGroup.ForEach(delegate (IMyMotorSuspension wheel) {
                double aux = i;
                String PreFix = wheel.CustomName.Split('-')[0];
                if (PreFix == "Right")
                {
                    aux = i * -1;
                }
                wheel.SetValue("Propulsion override", (Single)aux);
            });
        }
        public void SteeringOverride(double i)
        {
            this.wheelGroup.ForEach(delegate (IMyMotorSuspension wheel) {
                String PostFix = wheel.CustomName.Split('-')[1];
                if (PostFix.Equals("1") || PostFix.Equals("3"))
                {
                    wheel.SetValue("Steer override", (Single)i);
                }
            });
        }         
    }
    public class Util {
        public static Vector2D Vector3DToVector2D(Vector3D v)
        {
            return new Vector2D(v.X, v.Z);
        }

        public static double AngleBetween(Vector2D v1, Vector2D v2)
        {
            //Dotproduct
            double dotProduct = Vector2D.Dot(v1,v2);

            //Magnitude
            Func<double, double, double> Hypotenusa = (double x, double y) => Math.Sqrt(x * x + y * y);
            double dirInicialHypo = Hypotenusa(v1.X, v1.Y);
            double dirActualHypo = Hypotenusa(v2.X, v2.Y);

            // Angle
            double angleRad = Math.Acos((dotProduct) / (dirInicialHypo * dirActualHypo));
            double angleDegree = angleRad * (180 / Math.PI) ;

            return angleDegree;
        }
    }
}