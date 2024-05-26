namespace Script
{
    public enum OpCode
    {
        // basic stack operations
        PUSH,               // Push an object onto the stack
        POP,                // Pop a value from the stack
        DUP,                // Duplicate the object ontop of the stack

        // basic math operations
        ADD,                // Add the top two values on the stack
        SUB,                // Subtract the top two values on the stack
        MUL,                // Multiply the top two values on the stack
        DIV,                // Divide the top two values on the stack

        // call system defined function
        SYS_CALL,           // Call a system function identified by an operand

        // string operations
        CONCAT,             // Concatenate the top two values on the stack

        // label operations
        DEF_LABEL,          // Setup a label to jump back to later on

        // jump & jump statements (for labels)
        JUMP_LABEL,         // Jump to a past defined label
        JUMP_LABEL_IF,      // Jump if the top value on the stack is an operand
        JUMP_LABEL_IF_GE,   // Jump if the top value on the stack is >=
        JUMP_LABEL_IF_NGE,  // Jump if the top value on the stack is <
        JUMP_LABEL_IF_NEG,  // Jump if the top value on the stack is negative

        // jump & jump statements
        JUMP,               // Jump to a specific bytecode index
        JUMP_IF,            // Jump if the top value on the stack is an operand
        JUMP_IF_GE,         // Jump if the top value on the stack is >=
        JUMP_IF_NGE,        // Jump if the top value on the stack is <
        JUMP_IF_NEG,        // Jump if the top value on the stack is negative
    }
}