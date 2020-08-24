using Godot;
using System;

namespace TestAI
{
    public class InitialStartup : AILink
    {
        public InitialStartup(MasterController controller) : base(controller)
        {

        }
        private int count = 100;
        public override void StartingLink()
        {
            GD.Print("Loading");
        }
        public override void LinkUpdate()
        {
            count--;
        }
        public override bool LinkEnd()
        {
            return count < 0;
        }
        public override void Transition()
        {
            GD.Print("Transitioning");
            controller.UpdateLink(null);
        }
    }

}
