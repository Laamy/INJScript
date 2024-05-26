#pragma once

namespace Script {
    enum iOpcode
    {
        // basic stack operations
        PUSH = 0x00,                 // Push an object onto the stack
        POP,                        // Pop a value from the stack
        DUP,                        // Duplicate the object ontop of the stack

        // other
        NOP = 0x10,                 // Used to do nothing, common replacement for optimizations
        INT3,                       // Similar to NOP3 but to space things apart (not meant to be executed by interpreter)

        // basic math operations
        ADD = 0x20,                 // Add the top two values on the stack
        SUB,                        // Subtract the top two values on the stack
        MUL,                        // Multiply the top two values on the stack
        DIV,                        // Divide the top two values on the stack

        // call system defined function
        SYS_CALL = 0x30,            // Call a system function identified by an operand

        // string operations
        CONCAT = 0x40,              // Concatenate the top two values on the stack

        // label operations
        DEF_LABEL = 0x50,           // Setup a label to jump back to later on

        // jump & jump statements (for labels)
        JUMP_LABEL = 0x70,          // Jump to a past defined label (Call .Optimize() before running if for release)
        JUMP_LABEL_IF,              // Jump if the top value on the stack is an operand (Call .Optimize() before running if for release)
        JUMP_LABEL_IF_GE,           // Jump if the top value on the stack is ">=" (Call .Optimize() before running if for release)
        JUMP_LABEL_IF_NGE,          // Jump if the top value on the stack is "<" (Call .Optimize() before running if for release)
        JUMP_LABEL_IF_NEG,          // Jump if the top value on the stack is negative (Call .Optimize() before running if for release)

        // jump & jump statements
        JUMP = 0x60,                // Jump to a specific bytecode index
        JUMP_IF,                    // Jump if the top value on the stack is an operand
        JUMP_IF_GE,                 // Jump if the top value on the stack is ">="
        JUMP_IF_NGE,                // Jump if the top value on the stack is "<"
        JUMP_IF_NEG,                // Jump if the top value on the stack is negative
    };
}