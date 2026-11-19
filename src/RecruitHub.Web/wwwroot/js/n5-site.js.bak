// jQuery chỉ bổ trợ tương tác; quyền và dữ liệu luôn được kiểm tra tại máy chủ.
$(function () {
  $("form[data-confirm]").on("submit", function (event) {
    if (!window.confirm($(this).attr("data-confirm"))) event.preventDefault();
  });
  $('form[method="post"]').on("submit", function (event) {
    if (event.isDefaultPrevented() || !this.checkValidity()) return;
    const form = this;
    if ($(form).data("submitting")) {
      event.preventDefault();
      return;
    }
    $(form).data("submitting", true);
    // Chặn bấm lặp trong giao diện; CSDL vẫn có khóa duy nhất cho đơn ứng tuyển.
    setTimeout(
      () =>
        $(form)
          .find('button[type="submit"], button:not([type])')
          .prop("disabled", true),
      0,
    );
  });
  $(window).on("pageshow", function () {
    $("form").removeData("submitting").find("button").prop("disabled", false);
  });
});
