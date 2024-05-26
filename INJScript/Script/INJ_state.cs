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
                    Console.Write(interp.PopStack()); // out << peek
                });

                _state_interpreter.RegisterSystemFunction(0x11, interp => {
                    Console.WriteLine(interp.PopStack()); // out << peek << "\r\n"
                });

                _state_interpreter.RegisterSystemFunction(0x12, interp => {
                    Console.Clear(); // replace console buffer with empty ' ' spaces
                });

                _state_interpreter.RegisterSystemFunction(0x13, interp => {
                    char key = Console.ReadKey().KeyChar; // read key in console
                    _state_interpreter.Push(key); // push key onto stack
                });

                _state_interpreter.RegisterSystemFunction(0x14, interp => {
                    string key = Console.ReadLine(); // read line in console
                    _state_interpreter.Push(key); // push line onto stack
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

        public void Optimize()
        {
            List<Instruction> instructions = _state_interpreter.GetInstructions();
            List<Instruction> modified = instructions.ToArray().ToList();
            Dictionary<string, int> labels = new Dictionary<string, int>();

            // TODO: add a system to adjust pointers so I can easily remove bytes and adjust operand pointers

            // lets change some things which commonly slow down the scripts for example debugging information like labels!

            // first pre-processing
            {
                int ip = 0;
                foreach (Instruction instruction in instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OpCode.DEF_LABEL:
                            // for future processing
                            labels.Add((string)instruction.Operand, ip); // skip the nop for the split second boost

                            // replace with NOP
                            modified.Remove(instruction);
                            //modified[ip] = new Instruction(OpCode.NOP);
                            ip -= 1;
                            break;
                    }
                    ip++;
                }
            }

            instructions = modified.ToArray().ToList();

            // second pre-processing
            {
                int ip = 0;
                foreach (Instruction instruction in instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OpCode.JUMP_LABEL:
                            modified[ip] = new Instruction(OpCode.JUMP, labels[(string)instruction.Operand]);
                            break;
                        case OpCode.JUMP_LABEL_IF:
                            modified[ip] = new Instruction(OpCode.JUMP_IF, labels[(string)instruction.Operand]);
                            break;
                        case OpCode.JUMP_LABEL_IF_GE:
                            modified[ip] = new Instruction(OpCode.JUMP_IF_GE, labels[(string)instruction.Operand]);
                            break;
                        case OpCode.JUMP_LABEL_IF_NEG:
                            modified[ip] = new Instruction(OpCode.JUMP_IF_NEG, labels[(string)instruction.Operand]);
                            break;
                        case OpCode.JUMP_LABEL_IF_NGE:
                            modified[ip] = new Instruction(OpCode.JUMP_IF_NGE, labels[(string)instruction.Operand]);
                            break;
                    }
                    ip++;
                }
            }

            LoadBytecode(modified.ToArray());
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