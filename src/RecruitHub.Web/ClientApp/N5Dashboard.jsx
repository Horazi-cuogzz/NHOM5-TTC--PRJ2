import React, { useEffect, useState } from "react";
import { createRoot } from "react-dom/client";

const stages = [
  ["Submitted", "Đã nộp", "#2463eb"],
  ["Reviewing", "Đang xem xét", "#b77914"],
  ["Accepted", "Đạt yêu cầu", "#168560"],
  ["Rejected", "Từ chối", "#c54f62"],
];

function N5Dashboard() {
  const [data, setData] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [refresh, setRefresh] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setLoading(true);
    setError("");
    fetch("/api/employer/summary", {
      credentials: "same-origin",
      signal: controller.signal,
    })
      .then(async (response) => {
        if (response.status === 401)
          throw new Error(
            "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",
          );
        if (!response.ok)
          throw new Error("Chưa tải được tổng quan. Vui lòng thử lại.");
        return response.json();
      })
      .then(setData)
      .catch((err) => {
        if (err.name !== "AbortError") setError(err.message);
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort();
  }, [refresh]);
  if (loading)
    return (
      <div className="n5-dashboard-skeleton" role="status" aria-live="polite">
        <div></div>
        <div></div>
        <div></div>
        <p>Đang tải dữ liệu tuyển dụng…</p>
      </div>
    );
  if (error)
    return (
      <div className="n5-panel" role="alert">
        <h2>Chưa thể tải dữ liệu</h2>
        <p>{error}</p>
        <button
          className="btn btn-primary me-3"
          onClick={() => setRefresh((x) => x + 1)}
        >
          Thử lại
        </button>
        <a href="/tai-khoan/dang-nhap">Đăng nhập</a>
      </div>
    );
  if (!data.hasCompany)
    return (
      <div className="n5-empty">
        <span>◇</span>
        <h2>Hãy giới thiệu công ty của bạn</h2>
        <p>Hoàn thiện thông tin công ty trước khi bắt đầu đăng tin.</p>
        <a className="btn btn-primary" href="/cong-ty">
          Cập nhật công ty
        </a>
      </div>
    );
  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-4 gap-3">
        <strong>{data.companyName}</strong>
        <button
          className="btn btn-outline-primary"
          onClick={() => setRefresh((x) => x + 1)}
        >
          ↻ Làm mới
        </button>
      </div>
      <div className="n5-stats">
        <div className="n5-stat n5-stat-blue">
          <span>Tin đang mở</span>
          <strong>{data.openJobs}</strong>
          <small>Đã đăng và còn hạn nhận hồ sơ</small>
        </div>
        <div className="n5-stat">
          <span>Tổng đơn ứng tuyển</span>
          <strong>{data.totalApplications}</strong>
          <small>Tất cả đơn thuộc công ty</small>
        </div>
        <div className="n5-stat">
          <span>Cần tiếp nhận</span>
          <strong>{data.byStatus.Submitted || 0}</strong>
          <small>Hồ sơ chưa bắt đầu xem xét</small>
        </div>
      </div>
      <div className="n5-dashboard-grid">
        <section className="n5-panel">
          <h2>Đơn theo trạng thái</h2>
          <p className="text-muted">Theo dõi hồ sơ qua từng bước xử lý.</p>
          {data.totalApplications === 0 && (
            <p className="n5-inline-empty">
              Chưa có đơn ứng tuyển. Hãy đăng tin để kết nối ứng viên.
            </p>
          )}
          {stages.map(([key, label, color]) => (
            <div className="n5-bar-row" key={key}>
              <div>
                <span>{label}</span>
                <strong>{data.byStatus[key] || 0} đơn</strong>
              </div>
              <div className="n5-bar-track">
                <span
                  style={{
                    width:
                      ((data.byStatus[key] || 0) /
                        (data.totalApplications || 1)) *
                        100 +
                      "%",
                    background: color,
                  }}
                ></span>
              </div>
            </div>
          ))}
        </section>
        <section className="n5-panel">
          <h2>Tiếp tục công việc</h2>
          <a className="n5-quick-link" href="/don-ung-tuyen?status=Submitted">
            <span>
              <strong>Tiếp nhận hồ sơ mới</strong>
              <small>Xem những ứng viên vừa gửi đơn</small>
            </span>
            ↗
          </a>
          <a className="n5-quick-link" href="/nha-tuyen-dung/tin">
            <span>
              <strong>Quản lý tin tuyển dụng</strong>
              <small>Cập nhật, đăng và đóng tin</small>
            </span>
            ↗
          </a>
          <a className="n5-quick-link" href="/cong-ty">
            <span>
              <strong>Thông tin công ty</strong>
              <small>Giới thiệu doanh nghiệp với ứng viên</small>
            </span>
            ↗
          </a>
        </section>
      </div>
    </>
  );
}
createRoot(document.getElementById("n5-dashboard")).render(<N5Dashboard />);
