using System;
using System.Collections.Generic;

namespace Script
{
    public class StateDebugger
    {
        public event Action<Instruction, int, Stack<object>> InstructionTick;

        public void Trigger(Instruction instr, int ip, Stack<object> stack) => InstructionTick.Invoke(instr, ip, stack);
    }
}