#include <iostream>

#include "Script/INJ_state.h"

using namespace Script;

// place holders rq
INJ_state* inj_newthread(void* old) { return new INJ_state(); };

int main()
{
    INJ_state* state = inj_newthread(nullptr); // create new state

    state.LoadLibraries();

    state.LoadBytecode(nullptr);

    state.Run();
}