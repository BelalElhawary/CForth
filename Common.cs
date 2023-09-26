public class Common
{
    // built in functions
    public const string DUMP_FUNCTION = @";; -- dump --
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
";

    public const string HEADER = "format ELF64 executable\n";
    public const string CODE_SEGMENT = "segment readable executable\n";
    public const string ENTRY = "entry main\nmain:\n";
    public static string PUSH(object value) => @$";; -- push {value} --
    push   {value}
";
    public const string PLUS = @";; -- plus --
    pop    rax
    pop    rbx
    add    rax, rbx
    push   rax
";
    public const string MINUS = @";; -- minus --
    pop    rax
    pop    rbx
    sub    rbx, rax
    push   rax
";
    public const string EQUAL = @";; -- equal --
    mov    rcx, 0
    mov    rdx, 1
    pop    rax
    pop    rbx
    cmp    rax, rbx
    cmove  rcx, rdx
    push   rcx
";

    public const string NOT_EQUAL = @";; -- not equal --
    mov    rcx, 1
    mov    rdx, 0
    pop    rax
    pop    rbx
    cmp    rax, rbx
    cmove  rcx, rdx
    push   rcx
";
    public static string IF(object jmp) => @$";; -- if --
    pop    rax
    test   rax, rax
    jz     addr_{jmp}
";
    public static string ELSE(object ip, object jmp) => @$";; -- else --
    jmp    addr_{ip}
addr_{jmp}:
";
    public const string DUMP = @";; -- dump --
    pop    rdi
    call   dump
";
    public static string PROGRAM_RETURN(int returnCode) => @$"
    mov    rax, 60
    mov    rdi, {returnCode}
    syscall
";
}