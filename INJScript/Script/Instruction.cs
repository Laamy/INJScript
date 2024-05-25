namespace Script
{
    public class Instruction
    {
        public OpCode OpCode { get; }
        public object Operand { get; } // Optional operand

        public Instruction(OpCode opCode, object operand = null)
        {
            OpCode = opCode;
            Operand = operand;
        }
    }
}
