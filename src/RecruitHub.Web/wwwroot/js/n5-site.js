// RecruitHub Animation & Micro-interaction Engine (Inspired by Lucian UX/UI Prompt)
// Quyền và nghiệp vụ luôn được kiểm soát an toàn tại máy chủ.

$(function () {
  const rm = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // 1. Xác nhận thao tác qua data-confirm
  $("form[data-confirm]").on("submit", function (event) {
    if (!window.confirm($(this).attr("data-confirm"))) event.preventDefault();
  });

  // 2. Chống bấm kép & hiệu ứng Spin-to-loading cho nút bấm (Pattern 001)
  $('form[method="post"]').on("submit", function (event) {
    if (event.isDefaultPrevented() || !this.checkValidity()) return;
    const form = this;
    if ($(form).data("submitting")) {
      event.preventDefault();
      return;
    }
    $(form).data("submitting", true);
    const $btn = $(form).find('button[type="submit"], button:not([type])');
    $btn.addClass("n5-btn-loading");
    // Sử dụng pointer-events: none từ CSS n5-btn-loading để chống bấm kép an toàn tuyệt đối
  });

  $(window).on("pageshow", function () {
    $("form").removeData("submitting").find("button").prop("disabled", false).removeClass("n5-btn-loading");
    $("body").removeClass("n5-page-exit-back");
  });

  // 3. Tự động đóng thông báo flash sau 4.5 giây với hiệu ứng mờ dần
  const $alerts = $(".alert-success");
  if ($alerts.length) {
    setTimeout(function () {
      $alerts.fadeOut(400);
    }, 4500);
  }

  // 4. Hiệu ứng chuyển tiếp trang (Page Transitions khi bấm chi tiết & quay lại)
  $(document).on("click", ".n5-job-card a", function () {
    $(this).closest(".n5-job-card").addClass("n5-navigating");
  });

  $(document).on("click", "a.n5-back", function (e) {
    if (e.ctrlKey || e.metaKey || e.shiftKey || e.which === 2) return;
    const href = this.getAttribute("href");
    if (href && !href.startsWith("#") && !rm) {
      e.preventDefault();
      $("body").addClass("n5-page-exit-back");
      setTimeout(function () {
        window.location.href = href;
      }, 160);
    }
  });

  // 5. Pattern #216: Staggered Page Entrance (Áp dụng cho mọi trang web)
  if (!rm) {
    const staggerSelectors = [
      ".n5-hero-copy",
      ".n5-hero-art",
      ".n5-section-heading",
      ".n5-page-banner",
      ".n5-detail-header",
      ".n5-search",
      ".n5-content-grid > div",
      ".n5-panel",
      ".n5-apply-panel",
      ".n5-profile-aside",
      ".n5-table-wrap",
      ".n5-auth-wrap"
    ];

    let delayCounter = 40;
    staggerSelectors.forEach(function (sel) {
      $(sel).each(function () {
        if (!this.classList.contains("n5-stagger-item")) {
          this.style.setProperty("--n5-stagger-d", delayCounter);
          this.classList.add("n5-stagger-item");
          delayCounter = Math.min(delayCounter + 60, 480);
        }
      });
    });

    $(".n5-job-card").each(function (idx) {
      if (!this.classList.contains("n5-stagger-item")) {
        const d = Math.min(140 + idx * 55, 600);
        this.style.setProperty("--n5-stagger-d", d);
        this.classList.add("n5-stagger-item");
      }
    });
  }

  // 6. Pattern #205: Rippling Letter Wave cho tiêu đề Hero (Lucian #205)
  // Áp dụng cho 'Công việc phù hợp với kỹ năng của bạn.'
  const waveTargets = document.querySelectorAll(".n5-wave-text, .n5-hero-copy h1 em");
  waveTargets.forEach(function (target) {
    if (target.dataset.waveInit) return;
    target.dataset.waveInit = "true";
    const text = target.textContent.trim();
    target.innerHTML = "";
    target.classList.add("n5-wave-text");

    const letters = Array.from(text).map(function (ch) {
      const span = document.createElement("span");
      span.className = ch === " " ? "n5-wave-char is-space" : "n5-wave-char";
      span.textContent = ch;
      target.appendChild(span);
      return { el: span, x: 0, y: 0, v: 0, isSpace: ch === " " };
    });

    const SIGMA = 38;
    const AMP = 18;
    let px = null;
    let pulse = null;
    let raf = 0;

    function measure() {
      letters.forEach(function (l) {
        l.x = l.el.offsetLeft + l.el.offsetWidth / 2;
      });
    }

    function gaussian(dist, sig) {
      return Math.exp(-(dist * dist) / (2 * sig * sig));
    }

    function tick(now) {
      raf = 0;
      if (!target.isConnected) return;
      let moving = false;
      let pc = null;

      if (pulse) {
        const k = (now - pulse) / 1200;
        if (k >= 1) {
          pulse = null;
        } else {
          pc = -60 + (target.offsetWidth + 120) * k;
        }
      }

      letters.forEach(function (l) {
        if (l.isSpace) return;
        let tgt = 0;
        if (px !== null) tgt -= AMP * gaussian(l.x - px, SIGMA);
        if (pc !== null) tgt -= AMP * 1.3 * gaussian(l.x - pc, SIGMA * 1.25);

        l.v += (tgt - l.y) * 0.16;
        l.v *= 0.82;
        l.y += l.v;

        if (Math.abs(l.v) > 0.02 || Math.abs(tgt - l.y) > 0.05) moving = true;
        const t = Math.min(1, Math.max(0, -l.y / 24));
        l.el.style.setProperty("--y", l.y.toFixed(2) + "px");
        l.el.style.setProperty("--sy", (1 + Math.max(-0.06, Math.min(0.12, -l.v * 0.02))).toFixed(3));
        l.el.style.setProperty("--t", t.toFixed(3));
      });

      if (moving || pulse || px !== null) {
        raf = requestAnimationFrame(tick);
      }
    }

    function kick() {
      if (!raf && !rm) raf = requestAnimationFrame(tick);
    }

    target.addEventListener("pointermove", function (e) {
      if (!letters[0].x) measure();
      const rect = target.getBoundingClientRect();
      px = e.clientX - rect.left;
      kick();
    });

    target.addEventListener("pointerleave", function () {
      px = null;
      kick();
    });

    target.addEventListener("click", function () {
      measure();
      pulse = performance.now();
      kick();
    });

    // Tự động phát 1 đợt sóng sau khi vào trang 400ms
    setTimeout(function () {
      measure();
      pulse = performance.now();
      kick();
    }, 400);
  });

  // 7. Pattern #226: Grid-beam Hero Background cho HỒ SƠ KỸ NĂNG (Lucian #226)
  const asideSkillList = document.querySelectorAll(".n5-profile-aside");
  asideSkillList.forEach(function (aside) {
    const cv = aside.querySelector(".n5-grid-beam-cv");
    if (!cv) return;
    const cx = cv.getContext("2d");
    if (!cx) return;

    let W = aside.offsetWidth || 280;
    let H = aside.offsetHeight || 380;
    const CELL = 24;
    const dpr = Math.min(2, window.devicePixelRatio || 1);
    let beams = [];
    let acc = 0;
    let raf = 0;
    let last = 0;
    let visible = true;

    function resize() {
      W = aside.offsetWidth;
      H = aside.offsetHeight;
      cv.width = W * dpr;
      cv.height = H * dpr;
      cx.scale(dpr, dpr);
    }
    resize();
    $(window).on("resize", resize);

    // Bảng màu RecruitHub (Xanh dương, Lam ngọc, Xanh ngọc)
    const BEAM_COLORS = ["36, 99, 235", "14, 165, 233", "16, 185, 129"];

    function spawnBeam() {
      const isVertical = Math.random() < 0.5;
      const line = (1 + Math.floor(Math.random() * ((isVertical ? W : H) / CELL - 1))) * CELL - 1;
      const dir = Math.random() < 0.5 ? 1 : -1;
      beams.push({
        v: isVertical,
        l: line,
        p: dir > 0 ? -40 : (isVertical ? H : W) + 40,
        d: dir,
        sp: 70 + Math.random() * 80,
        len: 40 + Math.random() * 35,
        c: BEAM_COLORS[Math.floor(Math.random() * BEAM_COLORS.length)]
      });
    }

    function draw() {
      cx.clearRect(0, 0, W, H);
      beams.forEach(function (b) {
        const x0 = b.v ? b.l : b.p - b.d * b.len;
        const y0 = b.v ? b.p - b.d * b.len : b.l;
        const x1 = b.v ? b.l : b.p;
        const y1 = b.v ? b.p : b.l;
        const grad = cx.createLinearGradient(x0, y0, x1, y1);
        grad.addColorStop(0, "rgba(" + b.c + ", 0)");
        grad.addColorStop(1, "rgba(" + b.c + ", 0.85)");
        cx.strokeStyle = grad;
        cx.lineWidth = 1.6;
        cx.beginPath();
        cx.moveTo(x0 + 0.5, y0 + 0.5);
        cx.lineTo(x1 + 0.5, y1 + 0.5);
        cx.stroke();

        cx.fillStyle = "rgba(" + b.c + ", 1)";
        cx.shadowColor = "rgba(" + b.c + ", 0.9)";
        cx.shadowBlur = 6;
        cx.beginPath();
        cx.arc(x1 + 0.5, y1 + 0.5, 1.6, 0, Math.PI * 2);
        cx.fill();
        cx.shadowBlur = 0;
      });
    }

    function frame(now) {
      raf = 0;
      if (!aside.isConnected || !visible) return;
      const dt = Math.min(0.05, Math.max(0, (now - last) / 1000));
      last = now;
      acc += dt;
      if (acc > 0.55 && beams.length < 6) {
        acc = 0;
        spawnBeam();
      }
      beams.forEach(function (b) {
        b.p += b.d * b.sp * dt;
      });
      beams = beams.filter(function (b) {
        const limit = (b.v ? H : W) + 50;
        return b.d > 0 ? b.p < limit : b.p > -50;
      });
      draw();
      raf = requestAnimationFrame(frame);
    }

    function run() {
      if (!raf && visible && !rm) {
        last = performance.now();
        raf = requestAnimationFrame(frame);
      }
    }

    if ("IntersectionObserver" in window) {
      new IntersectionObserver(function (entries) {
        visible = entries[0].isIntersecting;
        if (visible) run();
      }).observe(aside);
    }

    // Spotlight theo con trỏ chuột
    aside.addEventListener("pointermove", function (e) {
      const rect = aside.getBoundingClientRect();
      aside.style.setProperty("--mx", (e.clientX - rect.left) + "px");
      aside.style.setProperty("--my", (e.clientY - rect.top) + "px");
    });

    aside.addEventListener("pointerleave", function () {
      aside.style.setProperty("--mx", "-200px");
      aside.style.setProperty("--my", "-200px");
    });

    // Khởi tạo trước 2 vệt sáng
    for (let i = 0; i < 2; i++) {
      spawnBeam();
      beams[i].p = (beams[i].v ? H : W) * (0.3 + i * 0.3);
    }
    draw();
    run();
  });

  // 8. Pattern #230: Hiệu ứng rung đỏ khi nhập lỗi (Head-shake Validation - Lucian #230)
  $("form").on("invalid", "input, select, textarea", function () {
    const el = this;
    el.classList.remove("is-bad");
    void el.offsetWidth; // Force reflow
    el.classList.add("is-bad");
    const $form = $(el).closest("form");
    $form.removeData("submitting");
    $form.find(".n5-btn-loading").removeClass("n5-btn-loading");
  });

  $("form").on("submit", function (e) {
    const form = this;
    const invalidInputs = $(form).find(":invalid, .input-validation-error");
    if (invalidInputs.length > 0) {
      $(form).removeData("submitting");
      $(form).find(".n5-btn-loading").removeClass("n5-btn-loading");
      invalidInputs.each(function () {
        const el = this;
        el.classList.remove("is-bad");
        void el.offsetWidth;
        el.classList.add("is-bad");
      });
      invalidInputs.first().focus();
    }
  });

  $("input, select, textarea").on("input change", function () {
    $(this).removeClass("is-bad");
  });

  // 9. Pattern #001: Liquid Glass Button Refraction Sheen (Nút 'Tìm việc' kính thủy tinh lỏng)
  $(document).on("pointermove", ".btn-primary.n5-btn-liquid", function (e) {
    const rect = this.getBoundingClientRect();
    const lx = ((e.clientX - rect.left) / rect.width) * 100;
    const ly = ((e.clientY - rect.top) / rect.height) * 100;
    this.style.setProperty("--lx", lx.toFixed(1) + "%");
    this.style.setProperty("--ly", ly.toFixed(1) + "%");
  });

  // 10. Pattern #010: Popover Select Menu Dropdown ('Tất cả địa điểm')
  const $locPicker = $(".n5-location-picker");
  if ($locPicker.length) {
    const $locTrigger = $locPicker.find(".n5-location-trigger");
    const $locPopover = $locPicker.find(".n5-location-popover");
    const $locInput = $locPicker.find("#n5-location");
    const $locLabel = $locPicker.find(".n5-location-label");

    function openLocationPopover() {
      $locPicker.attr("data-popover", "true");
      $locTrigger.attr("aria-expanded", "true");
      $locPicker.closest(".n5-search").addClass("is-popover-open");
    }

    function closeLocationPopover() {
      $locPicker.attr("data-popover", "false");
      $locTrigger.attr("aria-expanded", "false");
      $locPicker.closest(".n5-search").removeClass("is-popover-open");
    }

    $locTrigger.on("click", function (e) {
      e.preventDefault();
      e.stopPropagation();
      const isOpen = $locPicker.attr("data-popover") === "true";
      if (isOpen) {
        closeLocationPopover();
      } else {
        openLocationPopover();
      }
    });

    $locPicker.on("click", ".n5-location-opt", function (e) {
      e.preventDefault();
      const val = $(this).attr("data-val") || "";
      const text = $(this).find("span").first().text().trim();
      $locInput.val(val);
      $locLabel.text(text || "Tất cả địa điểm");
      $locPicker.find(".n5-location-opt").removeClass("is-selected").find(".n5-opt-check").remove();
      $(this).addClass("is-selected").append('<span class="n5-opt-check" aria-hidden="true">✓</span>');
      if (val) {
        $locPicker.find(".n5-popover-clear").show();
      } else {
        $locPicker.find(".n5-popover-clear").hide();
      }
      closeLocationPopover();
      $locTrigger.focus();
    });

    $locPicker.on("click", ".n5-popover-clear", function (e) {
      e.preventDefault();
      e.stopPropagation();
      $locInput.val("");
      $locLabel.text("Tất cả địa điểm");
      $locPicker.find(".n5-location-opt").removeClass("is-selected").find(".n5-opt-check").remove();
      $locPicker.find('.n5-location-opt[data-val=""]').addClass("is-selected").append('<span class="n5-opt-check" aria-hidden="true">✓</span>');
      $(this).hide();
      closeLocationPopover();
      $locTrigger.focus();
    });

    $(document).on("click", function (e) {
      if (!$(e.target).closest(".n5-location-picker").length) {
        closeLocationPopover();
      }
    });

    $(document).on("keydown", function (e) {
      if (e.key === "Escape" && $locPicker.attr("data-popover") === "true") {
        closeLocationPopover();
        $locTrigger.focus();
      }
    });
  }

  // 11. Pattern #019: Spring Check Interest Chips & Bộ đếm kỹ năng đã chọn
  function updateSkillCounter() {
    const count = $('.n5-skill-picker input[type="checkbox"]:checked').length;
    const $counter = $("#n5-selected-count");
    if ($counter.length) {
      $counter.text(count);
    }
  }

  $(document).on("change", '.n5-skill-picker input[type="checkbox"]', function () {
    const $label = $(this).closest("label");
    if (this.checked) {
      $label.addClass("is-checked");
      const span = $label.find("span")[0];
      if (span) {
        span.style.animation = "none";
        void span.offsetWidth; // reflow
        span.style.animation = "";
      }
    } else {
      $label.removeClass("is-checked");
    }
    updateSkillCounter();
  });

  // 12. Pattern #002: Magnetic Arrow Button (Hệ nút bấm hút con trỏ chuột & mũi tên trượt)
  if (!rm && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
    const magneticSelector = ".btn, .n5-detail-link";
    $(document).on("pointermove", magneticSelector, function (e) {
      const btn = this;
      const rect = btn.getBoundingClientRect();
      const centerX = rect.left + rect.width / 2;
      const centerY = rect.top + rect.height / 2;
      const dx = e.clientX - centerX;
      const dy = e.clientY - centerY;

      const moveX = Math.max(-8, Math.min(8, dx * 0.16));
      const moveY = Math.max(-6, Math.min(6, dy * 0.16));
      btn.style.transform = `translate(${moveX.toFixed(1)}px, ${moveY.toFixed(1)}px)`;

      const arrow = btn.querySelector(".n5-btn-arrow, [aria-hidden='true']");
      if (arrow) {
        const arrowX = moveX * 0.5 + 4;
        const arrowY = moveY * 0.4;
        arrow.style.transform = `translate(${arrowX.toFixed(1)}px, ${arrowY.toFixed(1)}px)`;
      }
    });

    $(document).on("pointerleave", magneticSelector, function () {
      const btn = this;
      btn.style.transform = "";
      const arrow = btn.querySelector(".n5-btn-arrow, [aria-hidden='true']");
      if (arrow) arrow.style.transform = "";
    });
  }

  // 13. Sắp xếp tin có tỷ lệ % cao hơn lên đầu tiên (Highest Match % First)
  function sortJobCardsByScore() {
    $(".n5-job-list").each(function () {
      const $list = $(this);
      const $cards = $list.children(".n5-job-card");
      if ($cards.length > 1) {
        const hasScores = $cards.toArray().some(function (card) {
          const s = parseFloat(card.getAttribute("data-score") || "-1");
          return s >= 0;
        });
        if (hasScores) {
          const sorted = $cards.toArray().sort(function (a, b) {
            const sa = parseFloat(a.getAttribute("data-score") || "-1");
            const sb = parseFloat(b.getAttribute("data-score") || "-1");
            return sb - sa;
          });
          $list.append(sorted);
        }
      }
    });
  }
  sortJobCardsByScore();

  // 14. Hiệu ứng quét vòng tròn kỹ năng từ 0 đến % thực tế + Bộ đếm số tăng dần
  if ("IntersectionObserver" in window) {
    const ringObserver = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            const ring = entry.target;
            const targetScore = parseFloat(ring.getAttribute("data-target") || ring.getAttribute("data-score") || 0);
            const $num = $(ring).find(".n5-match-num, > span");

            if (!ring.dataset.animated) {
              ring.dataset.animated = "true";
              if (isNaN(targetScore) || targetScore <= 0) {
                ring.style.setProperty("--score", 0);
                if ($num.length) $num.html('0<small>%</small>');
                return;
              }

              const cardIdx = $(ring).closest(".n5-job-card").index() || 0;
              const delay = rm ? 0 : Math.min(cardIdx * 80, 400);

              setTimeout(function () {
                const duration = rm ? 0 : 950;
                const startTime = performance.now();

                function step(now) {
                  const elapsed = now - startTime;
                  const progress = Math.min(elapsed / duration, 1);
                  const ease = progress === 1 ? 1 : 1 - Math.pow(2, -10 * progress);
                  const currentVal = targetScore * ease;

                  ring.style.setProperty("--score", currentVal.toFixed(1));
                  if ($num.length) {
                    $num.html(Math.round(currentVal) + '<small>%</small>');
                  }

                  if (progress < 1) {
                    requestAnimationFrame(step);
                  } else {
                    ring.style.setProperty("--score", targetScore);
                    if ($num.length) {
                      $num.html(Math.round(targetScore) + '<small>%</small>');
                    }
                    ring.classList.add("is-complete");
                  }
                }

                if (rm) {
                  ring.style.setProperty("--score", targetScore);
                  if ($num.length) $num.html(Math.round(targetScore) + '<small>%</small>');
                  ring.classList.add("is-complete");
                } else {
                  requestAnimationFrame(step);
                }
              }, delay);
            }
            ringObserver.unobserve(ring);
          }
        });
      },
      { threshold: 0.15 }
    );

    $(".n5-match-ring").each(function () {
      const $r = $(this);
      let target = parseFloat($r.attr("data-target") || 0);
      if (!target) {
        const match = ($r.attr("style") || "").match(/--score:\s*([\d.]+)/);
        target = match ? parseFloat(match[1]) : 0;
      }
      $r.attr("data-target", target);
      this.style.setProperty("--score", 0);
      ringObserver.observe(this);
    });
  }

  // 12. Hiệu ứng 3D Card Deck Gallery Mouse Parallax & Quick Filter (Tích hợp từ dự án cũ)
  const heroDeck = document.getElementById("n5-hero-deck");
  if (heroDeck && !rm) {
    const cards = heroDeck.querySelectorAll(".cdg-card");
    let currentX = 0, currentY = 0;
    let targetX = 0, targetY = 0;
    let rafId = null;
    let isHovering = false;

    // Trọng số parallax theo từng tầng độ sâu z-axis để tạo cảm giác 3D tự nhiên
    const layerWeights = [
      { px: 14, py: 10, rx: 6,  ry: 8  }, // Card 1 (tz: -150)
      { px: 18, py: 12, rx: 7,  ry: 9  }, // Card 2 (tz: -120)
      { px: 22, py: 14, rx: 8,  ry: 10 }, // Card 3 (tz: -75)
      { px: 26, py: 16, rx: 8,  ry: 11 }, // Card 4 (tz: -45)
      { px: 30, py: 18, rx: 9,  ry: 12 }, // Card 5 (tz: -20)
      { px: 38, py: 24, rx: 11, ry: 14 }, // Card 6 (Hero - front z: 5)
    ];

    function updateParallax() {
      // Nội suy tuyến tính mượt mà (smooth lerp 0.08)
      currentX += (targetX - currentX) * 0.08;
      currentY += (targetY - currentY) * 0.08;

      cards.forEach(function (card, index) {
        const w = layerWeights[index] || layerWeights[0];
        const px = (currentX * w.px).toFixed(2);
        const py = (currentY * w.py).toFixed(2);
        const prx = (-currentY * w.rx).toFixed(2);
        const pry = (currentX * w.ry).toFixed(2);

        card.style.setProperty("--px", px + "px");
        card.style.setProperty("--py", py + "px");
        card.style.setProperty("--prx", prx + "deg");
        card.style.setProperty("--pry", pry + "deg");
      });

      const dist = Math.abs(targetX - currentX) + Math.abs(targetY - currentY);
      if (isHovering || dist > 0.001) {
        rafId = requestAnimationFrame(updateParallax);
      } else {
        rafId = null;
      }
    }

    const heroSection = heroDeck.closest(".n5-hero") || heroDeck;

    heroSection.addEventListener("pointermove", function (e) {
      const rect = heroDeck.getBoundingClientRect();
      if (!rect.width || !rect.height) return;
      targetX = ((e.clientX - rect.left) / rect.width - 0.5) * 2;
      targetY = ((e.clientY - rect.top) / rect.height - 0.5) * 2;
      targetX = Math.max(-1.25, Math.min(1.25, targetX));
      targetY = Math.max(-1.25, Math.min(1.25, targetY));

      isHovering = true;
      if (!rafId) {
        rafId = requestAnimationFrame(updateParallax);
      }
    });

    heroSection.addEventListener("pointerleave", function () {
      targetX = 0;
      targetY = 0;
      isHovering = false;
      if (!rafId) {
        rafId = requestAnimationFrame(updateParallax);
      }
    });

    // Nhấp vào thẻ 3D để lọc nhanh việc làm theo từ khóa
    cards.forEach(function (card) {
      card.addEventListener("click", function () {
        const query = this.getAttribute("data-query");
        if (query) {
          window.location.href = "/viec-lam?query=" + encodeURIComponent(query);
        }
      });
    });
  }
});
