/* Keystone Assembler Engine */
/* By Nguyen Anh Quynh, 2016-2018 */

#ifndef KEYSTONE_EVM_H
#define KEYSTONE_EVM_H

#ifdef __cplusplus
extern "C" {
#endif

#include "keystone.h"

typedef enum ks_err_asm_evm {
    KS_ERR_ASM_EVM_INVALIDOPERAND = KS_ERR_ASM_ARCH,
    KS_ERR_ASM_EVM_MISSINGFEATURE,
    KS_ERR_ASM_EVM_MNEMONICFAIL,
} ks_err_asm_evm;

#ifdef __cplusplus
}
#endif

#endif
