namespace Script
{
    public class Instruction
    {
        public OpCode OpCode { get; }
        public int Operand { get; } // Optional operand

        public Instruction(OpCode opCode, int operand = 0)
        {
            OpCode = opCode;
            Operand = operand;
        }
    }
}
