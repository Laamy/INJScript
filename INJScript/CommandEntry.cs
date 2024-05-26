using Script;

using System;

public class CommandEntry
{
    static void Main(string[] args)
    {
        INJ_state state = new INJ_state();

        state.LoadLibraries(); // load in system functions

        //state.AttachDebugger();

        state.LoadBytecode(new Instruction[]
            {
                new Instruction(OpCode.PUSH, ">"),
                new Instruction(OpCode.SYS_CALL, 0x10), // write

                new Instruction(OpCode.SYS_CALL, 0x14), // readline

                new Instruction(OpCode.SYS_CALL, 0x11), // writeline
            });

        // optimize for live running (does not work well with debugger as it removes labels)
        state.Optimize();

        /*
         
         
        state.LoadBytecode(new Instruction[]
            {
                new Instruction(OpCode.PUSH, 0),

                new Instruction(OpCode.DEF_LABEL, "START"),

                new Instruction(OpCode.PUSH, 1),
                new Instruction(OpCode.ADD),

                new Instruction(OpCode.DUP), // duplicate
                new Instruction(OpCode.PUSH, " is the result"),
                new Instruction(OpCode.CONCAT), // contact num & string
                new Instruction(OpCode.SYS_CALL, 0x11), // print
                //new Instruction(OpCode.POP), // pop useless stuff

                new Instruction(OpCode.PUSH, 9), // goal
                new Instruction(OpCode.JUMP_LABEL_IF_NGE, "START"), // loop if (not-greater-equal) goal

                new Instruction(OpCode.PUSH, "Script has halted"),
                new Instruction(OpCode.SYS_CALL, 0x11), // print message saying script ended
            });
         
         */

        /*
        
        state.LoadBytecode(new Instruction[]
            {
                // push 1 & 2 onto the stack then add them together (1) with (2)
                new Instruction(OpCode.PUSH, 1),
                new Instruction(OpCode.PUSH, 2),
                new Instruction(OpCode.ADD),

                // push the string of " is the result" and combine the maths from earlier with it
                new Instruction(OpCode.PUSH, " is the result"),
                new Instruction(OpCode.CONCAT),

                // print the current thing ontop of the stack (in this cast it would be "3 is the result"
                new Instruction(OpCode.SYS_CALL, 0x11),

                // restart loop from bytecode 0 (while loop)
                new Instruction(OpCode.JUMP, 0)
            });

         */

        state.Run();

        Console.ReadKey();
    }
}