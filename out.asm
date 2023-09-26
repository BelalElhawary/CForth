format ELF64 executable
segment readable executable
;; -- dump --
dump:
    mov    r8, -3689348814741910323
    sub    rsp, 40
    mov    BYTE [rsp+31], 10
    lea    rcx, [rsp+30]
.L2:
    mov    rax, rdi
    mul    r8
    mov    rax, rdi
    shr    rdx, 3
    lea    rsi, [rdx+rdx*4]
    add    rsi, rsi
    sub    rax, rsi
    mov    rsi, rcx
    sub    rcx, 1
    add    eax, 48
    mov    BYTE [rcx+1], al
    mov    rax, rdi
    mov    rdi, rdx
    cmp    rax, 9
    ja     .L2
    lea    rdx, [rsp+32]
    mov    edi, 1
    xor    eax, eax
    sub    rdx, rsi
    mov    rax, 1
    syscall
    add    rsp, 40
    ret
entry main
main:
;; -- push 10 --
    push   10
;; -- push 10 --
    push   10
;; -- equal --
    mov    rcx, 0
    mov    rdx, 1
    pop    rax
    pop    rbx
    cmp    rax, rbx
    cmove  rcx, rdx
    push   rcx
;; -- if --
    pop    rax
    test   rax, rax
    jz     addr_0
;; -- push 0 --
    push   0
;; -- if --
    pop    rax
    test   rax, rax
    jz     addr_8
;; -- push 100 --
    push   100
;; -- if --
    pop    rax
    test   rax, rax
    jz     addr_10
;; -- push 200 --
    push   200
;; -- else --
    jmp    addr_0
addr_10:
;; -- push 200 --
    push   200

    mov    rax, 60
    mov    rdi, 0
    syscall
