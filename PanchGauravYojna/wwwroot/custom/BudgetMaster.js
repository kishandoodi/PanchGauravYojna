document.addEventListener("DOMContentLoaded", function () {

    bindGarauv();
    bindDistrict();
    document.getElementById("DistrictId")
        .addEventListener("change", onDistrictChange);

    document.body.classList.add("vetting-active");

    const modalEl = document.getElementById("verifyModalvetting");
    const verifyModal = new bootstrap.Modal(modalEl);

   

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

document.addEventListener("click", function (e) {

    const btn = e.target.closest(".verify-btn-vetting");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-gauravid");
    const DistrictId = btn.getAttribute("data-districtid");
    const SubQuestionMasterId = btn.getAttribute("data-subquestionmasterid");
    const QuestionMasterId = btn.getAttribute("data-questionmasterid");

    ajax.doPostAjaxHtml(
        "/Budget/GetPendingVettingList",
        {
            RawId: rawid,
            garauvId: gauravid,
            DistrictId: DistrictId,
            SubQuestionMasterId: SubQuestionMasterId,
            QuestionMasterId: QuestionMasterId
        },
        function (response) {

            // LEFT readonly table
            document.getElementById("readonlyData").innerHTML = response;

            // RIGHT editable version
            document.getElementById("editableData").innerHTML = response;

            // RIGHT side cells ko input me convert karo
            document
                .querySelectorAll("#editableData tr")
                .forEach(tr => {

                    tr.querySelectorAll("td").forEach((td, index) => {

                        // index 0 = serial number column
                        if (index === 0) return;

                        // verify button wali cell skip
                        if (td.querySelector("button")) return;

                        const text = td.innerText.trim();
                        if (!text) return;

                        td.innerHTML =
                            `<input class="form-control edit-field" data-col="${index}" value="${text}">`;
                    });

                });


            const modal = new bootstrap.Modal(
                document.getElementById("verifyModalvetting")
            );
            modal.show();
        }
    );
});

document.getElementById("saveVerify")
    .addEventListener("click", function () {

        let data = [];

        document
            .querySelectorAll("#editableData tr")
            .forEach(tr => {

                const rowId = document
                    .querySelector(".verify-btn-vetting")
                    ?.getAttribute("data-rowid");

                tr.querySelectorAll(".edit-field")
                    .forEach((input, index) => {

                        data.push({
                            RowId: rowId,
                            ColumnIndex: index,
                            Value: input.value
                        });
                    });
            });

        ajax.doPostAjax(
            "/Budget/SaveVettingData",
            { items: data },
            function () {
                alert("Saved successfully");
                location.reload();
            }
        );
    });

