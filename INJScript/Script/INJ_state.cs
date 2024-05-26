using System;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
    /// <summary>
    /// Wrapper for InjectScript BytecodeInterpreter
    /// </summary>
    public class INJ_state
    {
        private InjScrptInterpret _state_interpreter;
        private StateDebugger _state_debugger;

        /// <summary>
        /// Create a new InjectScript state
        /// </summary>
        public INJ_state()
        {
            _state_interpreter = new InjScrptInterpret();
        }

        /// <summary>
        /// Load Instruction set into states interpreter
        /// </summary>
        /// <param name="script">Instructions to load</param>
        public void LoadBytecode(Instruction[] script)
        {
            // Load instructions into the interpreter
            _state_interpreter.LoadInstructions(script.ToList());
        }

        /// <summary>
        /// Register the default pre-prepared system functions
        /// </summary>
        public void LoadLibraries()
        {
            // Register default system functions

            { // CONSOLE
                _state_interpreter.RegisterSystemFunction(0x10, interp => {
                    Console.Write(interp.PeekStack()); // out << peek
                });

                _state_interpreter.RegisterSystemFunction(0x11, interp => {
                    Console.WriteLine(interp.PeekStack()); // out << peek << "\r\n"
                });

                _state_interpreter.RegisterSystemFunction(0x12, interp => {
                    Console.Clear(); // replace console buffer with empty ' ' spaces
                });

                _state_interpreter.RegisterSystemFunction(0x13, interp => {
                    char key = Console.ReadKey().KeyChar; // read key in console
                    _state_interpreter.Push(key); // push key onto stack
                });
            }
        }

        /// <summary>
        /// Start executing InjectScript Instructions live
        /// </summary>
        public void Run()
        {
            _state_interpreter.Execute();
        }

        /// <summary>
        /// Cause State to shutdown (wipes when LoadBytecode is called)
        /// </summary>
        public void Reset()
        {
            _state_interpreter.Halt();
        }

        /// <summary>
        /// Attach a basic information debugger to the state.
        /// lets you step through instructions
        /// </summary>
        public void AttachDebugger()
        {
            if (_state_debugger != null)
                throw new Exception("Debugger already attached!");

            _state_debugger = new StateDebugger();
            _state_debugger.InstructionTick += OnTick;

            _state_interpreter.Attach(_state_debugger);
        }

        private void OnTick(Instruction instruction, int ip, Stack<object> stack)
        {
            Console.WriteLine($"{ip}: {instruction.OpCode.ToString()}, {instruction.Operand}, S: {string.Join(", ", stack.Reverse())}");
            Console.ReadKey();
        }
    }
}