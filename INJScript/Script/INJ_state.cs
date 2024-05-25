using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Script
{
    public class INJ_state
    {
        private BytecodeInterpreter _state_interpreter;

        public INJ_state()
        {
            _state_interpreter = new BytecodeInterpreter();
        }

        public void LoadBytecode(Instruction[] script)
        {
            // Load instructions into the interpreter
            _state_interpreter.LoadInstructions(script.ToList());
        }

        public void LoadLibraries()
        {
            // Register default system functions
            _state_interpreter.RegisterSystemFunction(0x10, interp => { // std::cout
                Console.Write(interp.PeekStack());
            });

            _state_interpreter.RegisterSystemFunction(0x11, interp => { // std::cout << '\n'
                Console.WriteLine(interp.PeekStack());
            });
        }

        public void Run()
        {
            _state_interpreter.Execute();
        }

        public void Reset()
        {
            _state_interpreter = new BytecodeInterpreter();
        }
    }
}