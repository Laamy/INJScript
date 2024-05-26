#pragma once

namespace Script {
	class Instruction
	{
	public: // variables
		iOpcode OpCode;
		std::any Operand;

	public: // methods
		Instruction(iOpcode OpCode, std::any Operand = NULL) {
			this->OpCode = OpCode;
			this->Operand = Operand;
		}
	};
}