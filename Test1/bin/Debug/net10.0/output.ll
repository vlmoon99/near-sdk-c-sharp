; ModuleID = 'simple_module'
source_filename = "simple_module"
target datalayout = "e-m:e-p:32:32-i64:64-n32:64-S128"
target triple = "wasm32-unknown-unknown"

@.str = private unnamed_addr constant [11 x i8] c"hello world"

declare void @log_utf8(i64, i64)

define void @helloworld() {
entry:
  call void @log_utf8(i64 11, i64 ptrtoint (ptr @.str to i64))
  ret void
}
