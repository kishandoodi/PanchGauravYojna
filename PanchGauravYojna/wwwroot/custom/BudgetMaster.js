document.addEventListener("DOMContentLoaded", function () {

    bindGarauv();
    bindDistrict();
    document.getElementById("DistrictId")
        .addEventListener("change", onDistrictChange);

    document.body.classList.add("vetting-active");

    const modalEl = document.getElementById("verifyModalvetting");
    const verifyModal = new bootstrap.Modal(modalEl);

    //let currentRow = null;

    //// verify click
    //document.addEventListener("click", function (e) {

    //    if (!e.target.classList.contains("verify-btn")) return;

    //    currentRow = e.target.closest("tr");

    //    var cols = JSON.parse(e.target.dataset.columns);

    //    const columnMap = [
    //        { id: 17, label: "गतिविधि" },
    //        { id: 1, label: "लागत" },
    //        { id: 2, label: "स्वीकृत बजट" }
    //    ];

    //    let readonlyHtml = "";
    //    let editableHtml = "";

    //    columnMap.forEach(col => {
    //        let value = cols[col.id] || "";

    //        readonlyHtml += `
    //            <div class="mb-2">
    //                <label>${col.label}</label>
    //                <input class="form-control" value="${value}" readonly>
    //            </div>`;

    //        editableHtml += `
    //            <div class="mb-2">
    //                <label>${col.label}</label>
    //                <input class="form-control edit-field"
    //                       data-col="${col.id}"
    //                       value="${value}">
    //            </div>`;
    //    });

    //    document.getElementById("readonlyData").innerHTML = readonlyHtml;
    //    document.getElementById("editableData").innerHTML = editableHtml;

    //    verifyModal.show();
    //});
    // verify click
    document.getElementById("verify-btn-vetting")
        .addEventListener("click", function () {
            const id = this.getAttribute("data-rowid");
            const id = this.getAttribute("data-rowid");
            const id = this.getAttribute("data-rowid");
        });

    // save click
    document.getElementById("saveVerify")
        .addEventListener("click", function () {

            if (!currentRow) return;

            const inputs = document.querySelectorAll(".edit-field");

            let values = {};
            inputs.forEach(i => {
                values[i.dataset.col] = i.value;
            });

            currentRow.children[1].innerText = values[17] || "";
            currentRow.children[2].innerText = values[1] || "";
            currentRow.children[3].innerText = values[2] || "";

            verifyModal.hide();
        });

    // cleanup
    //modalEl.addEventListener("hidden.bs.modal", function () {
    //    document.querySelectorAll(".modal-backdrop").forEach(b => b.remove());
    //    document.body.classList.remove("modal-open");
    //    document.body.style.removeProperty("padding-right");
    //});

});
function onDistrictChange() {

    var garauvId = document.getElementById("GarauvId").value;
    var districtId = document.getElementById("DistrictId").value;
    if (!garauvId) return;
    if (!districtId) return;

    ajax.doPostAjaxHtml(
        "/Budget/GetVettingList",
        { garauvId: garauvId, districtId: districtId },
        function (response) {
            document.getElementById("vettingContainer").innerHTML = response;
            
        }
    );
}
function bindGarauv() {
    common.BindDropdown("/Budget/BindGauravDropDown", "GarauvId", "Garauv", 0);
    setTimeout(function () {
        fixGarauvDropdown();
    }, 500);
}
function fixGarauvDropdown() {
    var ddl = document.getElementById("GarauvId");

    if (!ddl) return;

    // agar sirf 1 actual entry hai
    if (ddl.options.length === 2) {
        ddl.remove(0); // "Select Garauv" remove
        ddl.selectedIndex = 0;
    }
}
function bindDistrict() {
    common.BindDropdown("/Budget/BindDistrictDropDownAll","DistrictId","District",0);
}

// ---- Bootstrap modal permanent cleanup ----
document.addEventListener('hidden.bs.modal', function () {
    document.querySelectorAll('.modal-backdrop')
        .forEach(e => e.remove());

    document.body.classList.remove('modal-open');
});
