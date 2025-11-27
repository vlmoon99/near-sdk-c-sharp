#include <cstdint>

extern "C" int32_t __imported_wasi_snapshot_preview1_args_get(int32_t* arg0, int32_t* arg1) {
    *arg0 = 0;
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_args_sizes_get(int32_t* arg0, int32_t* arg1) {
    *arg0 = 100;
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_environ_get(int32_t* arg0, int32_t* arg1) {
    *arg0 = 0;
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_environ_sizes_get(int32_t* arg0, int32_t* arg1) {
    *arg0 = 0;
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_clock_time_get(int32_t arg0,int64_t arg1, int32_t arg2) {
    arg0 = 0;
    arg1 = 0;
    arg2 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_close(int32_t arg0) {
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_fdstat_get(int32_t arg0, int32_t* arg1) {
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_fdstat_set_flags(int32_t arg0, int32_t arg1) {
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_prestat_get(int32_t arg0, int32_t* arg1) {
    *arg1 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_prestat_dir_name(int32_t arg0, int32_t arg1, int32_t arg2) {
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_read(int32_t arg0, int32_t arg1, int32_t* arg2, int32_t arg3) {
    *arg2 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_seek(int32_t arg0, int64_t arg1, int32_t arg2, int32_t* arg3) {
    *arg3 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_fd_write(int32_t arg0, int32_t arg1, int32_t* arg2, int32_t arg3) {
    *arg2 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_path_open(int32_t arg0, int32_t arg1, int32_t arg2, int32_t arg3, int32_t arg4, int64_t arg5, int64_t arg6,int32_t arg7,int32_t *arg8) {
    *arg8 = 0;
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_poll_oneoff(int32_t arg0, int32_t arg1, int32_t* arg2, int32_t arg3) {
    *arg2 = 0;
    return 0;
}

extern "C" void __imported_wasi_snapshot_preview1_proc_exit(int32_t arg0) {
}

extern "C" int32_t __imported_wasi_snapshot_preview1_sched_yield() {
    return 0;
}

extern "C" int32_t __imported_wasi_snapshot_preview1_random_get(int32_t* arg0, int32_t arg1) {
    *arg0 = 0;
    return 0;
}

extern "C" int32_t __wasi_preview1_adapter_close_badfd(int32_t arg0) {
    return 0;
}

extern "C" void __wasm_import_poll_pollable_drop(int32_t arg0) {
}

extern "C" void __wasm_import_streams_input_stream_drop(int32_t arg0) {
}

extern "C" void __wasm_import_streams_output_stream_drop(int32_t arg0) {
}

extern "C" void __wasm_import_udp_udp_socket_drop(int32_t arg0) {}

extern "C" void __wasm_import_udp_incoming_datagram_stream_drop(int32_t arg0) {
}

extern "C" void __wasm_import_udp_outgoing_datagram_stream_drop(int32_t arg0) {
}

extern "C" void __wasm_import_tcp_tcp_socket_drop(int32_t arg0) {
}

int main(int argc, char* argv[])
{
    return argc;
}




