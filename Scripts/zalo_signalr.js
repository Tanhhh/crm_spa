$(document).ready(function () {
    //// Tạo kết nối SignalR cho hub 'zaloHub'
    //var hubProxyZalo = connection.createHubProxy('zaloHub');

    //// Bắt đầu kết nối SignalR cho hub 'zaloHub'
    //connectionZalo.start().done(function () {
    //    console.log('SignalR connected for zaloHub.');
    //}).fail(function (error) {
    //    console.error('SignalR connection error for zaloHub: ' + error);
    //});

    //// Định nghĩa hàm hiển thị modal từ Zalo với ID động
    //function showZaloNotificationModal(message, modalId) {
    //    $('#notificationMessageZalo' + modalId).text(message);
    //    $('#notificationModalZalo' + modalId).addClass('show');
    //}

    //// Xóa modal backdrop khi modal được hiển thị
    //$('[id^=notificationModalZalo]').on('show.bs.modal', function () {
    //    $('.modal-backdrop.in').remove();
    //});

    //// Xóa modal backdrop khi modal đã được hiển thị
    //$('[id^=notificationModalZalo]').on('shown.bs.modal', function () {
    //    $('.modal-backdrop').remove();
    //});

    //// Xử lý sự kiện khi người dùng nhấn nút đóng modal
    //$('[id^=closeModalZalo]').on('click', function () {
    //    var modalId = $(this).attr('data-modal-id');
    //    var modal = document.getElementById('notificationModalZalo' + modalId);
    //    modal.classList.remove('show');
    //});

    //// Đăng ký xử lý thông báo từ Zalo
    //hubProxyZalo.on('SendZaloNotification', function (message, modalId) {
    //    showZaloNotificationModal(message, modalId);
    //});
});
