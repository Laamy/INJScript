using Script;
using System;
using System.Collections.Generic;

public class CommandEntry
{
    static void Main(string[] args)
    {
        var interpreter = new BytecodeInterpreter();

        // Define some instructions
        var instructions = new List<Instruction>
        {
            new Instruction(OpCode.PUSH_INT, 1),         // Push 1
            new Instruction(OpCode.PUSH_INT, 2),         // Push 2
            new Instruction(OpCode.ADD),                 // Add top two values
            new Instruction(OpCode.PUSH_STRING, " is the result"), // Push string " is the result"
            new Instruction(OpCode.CONCAT),              // Concatenate top two values
            new Instruction(OpCode.SYS_CALL, 0x10),      // Print the result
            new Instruction(OpCode.JUMP, 0)              // Jump back to the start
        };

        // Load instructions into the interpreter
        interpreter.LoadInstructions(instructions);

        // Register system functions
        interpreter.RegisterSystemFunction(0x10, interp => {
            Console.WriteLine(interp.PeekStack());
        });

        // Execute the bytecode
        interpreter.Execute();

        Console.ReadKey();
    }
}