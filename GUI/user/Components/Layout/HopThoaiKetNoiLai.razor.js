// Set up event handlers
const reconnectModal = document.getElementById("components-reconnect-modal");
reconnectModal.addEventListener("components-reconnect-state-changed", xuLyTrangThaiKetNoi);

const retryButton = document.getElementById("components-reconnect-button");
retryButton.addEventListener("click", thuKetNoiLai);

const resumeButton = document.getElementById("components-resume-button");
resumeButton.addEventListener("click", tiepTucPhien);

function xuLyTrangThaiKetNoi(event) {
    if (event.detail.state === "show") {
        reconnectModal.showModal();
    } else if (event.detail.state === "hide") {
        reconnectModal.close();
    } else if (event.detail.state === "failed") {
        document.addEventListener("visibilitychange", thuLaiKhiHienTrang);
    } else if (event.detail.state === "rejected") {
        location.reload();
    }
}

async function thuKetNoiLai() {
    document.removeEventListener("visibilitychange", thuLaiKhiHienTrang);

    try {
        // Reconnect will asynchronously return:
        // - true to mean success
        // - false to mean we reached the server, but it rejected the connection (e.g., unknown circuit ID)
        // - exception to mean we didn't reach the server (this can be sync or async)
        const successful = await Blazor.reconnect();
        if (!successful) {
            // We have been able to reach the server, but the circuit is no longer available.
            // We'll reload the page so the user can continue using the app as quickly as possible.
            const resumeSuccessful = await Blazor.resumeCircuit();
            if (!resumeSuccessful) {
                location.reload();
            } else {
                reconnectModal.close();
            }
        }
    } catch (err) {
        // We got an exception, server is currently unavailable
        document.addEventListener("visibilitychange", thuLaiKhiHienTrang);
    }
}

async function tiepTucPhien() {
    try {
        const successful = await Blazor.resumeCircuit();
        if (!successful) {
            location.reload();
        }
    } catch {
        reconnectModal.classList.replace("components-reconnect-paused", "components-reconnect-resume-failed");
    }
}

async function thuLaiKhiHienTrang() {
    if (document.visibilityState === "visible") {
        await thuKetNoiLai();
    }
}
