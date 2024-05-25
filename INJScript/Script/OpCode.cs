namespace Script
{
    public enum OpCode
    {
        PUSH,   // Push a value onto the stack
        POP,    // Pop a value from the stack
        ADD,    // Add the top two values on the stack
        SUB,    // Subtract the top two values on the stack
        MUL,    // Multiply the top two values on the stack
        DIV,    // Divide the top two values on the stack
        SYS_CALL, // Call a system function identified by an operand
        JUMP,   // Jump to a specific bytecode index
        JUMP_IF_ZERO, // Jump if the top value on the stack is zero
        JUMP_IF_NEG,  // Jump if the top value on the stack is negative
    }
}