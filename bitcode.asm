	.text
	.file	"Main"
	.globl	main                    # -- Begin function main
	.p2align	4, 0x90
	.type	main,@function
main:                                   # @main
	.cfi_startproc
# %bb.0:                                # %entry
	movq	$69, -8(%rsp)
	movl	$1, %eax
	movl	$1, %edi
	movl	$69, %esi
	movl	$1, %edx
	#APP
	syscall
	#NO_APP
	movl	$69, %eax
	retq
.Lfunc_end0:
	.size	main, .Lfunc_end0-main
	.cfi_endproc
                                        # -- End function

	.section	".note.GNU-stack","",@progbits
