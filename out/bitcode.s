	.text
	.intel_syntax noprefix
	.file	"Main"
	.globl	main                    # -- Begin function main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
	.cfi_startproc
# %bb.0:                                # %entry
	mov	qword ptr [rsp - 8], 69
	lea	rsi, [rsp - 8]
	mov	eax, 1
	mov	edi, 1
	mov	edx, 1
	#APP
	syscall
	#NO_APP
	ret
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
	.cfi_endproc
                                        # -- End function

	.section	".note.GNU-stack","",@progbits
