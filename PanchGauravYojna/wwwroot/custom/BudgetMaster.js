document.addEventListener("DOMContentLoaded", function () {

    bindGarauv();
    bindDistrict();
    document.getElementById("DistrictId")
        .addEventListener("change", onDistrictChange);

    document.getElementById("GarauvId")
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

//document.addEventListener("click", function (e) {

//    const btn = e.target.closest(".verify-btn-vetting");
//    if (!btn) return;

//    const rawid = btn.getAttribute("data-rowid");
//    const gauravid = btn.getAttribute("data-gauravid");
//    const DistrictId = btn.getAttribute("data-districtid");
//    const SubQuestionMasterId = btn.getAttribute("data-subquestionmasterid");
//    const QuestionMasterId = btn.getAttribute("data-questionmasterid");

//    ajax.doPostAjaxHtml(
//        "/Budget/GetPendingVettingList",
//        {
//            RawId: rawid,
//            garauvId: gauravid,
//            DistrictId: DistrictId,
//            SubQuestionMasterId: SubQuestionMasterId,
//            QuestionMasterId: QuestionMasterId
//        },
//        function (response) {

//            // LEFT readonly table
//            document.getElementById("readonlyData").innerHTML = response;

//            // RIGHT editable version
//            document.getElementById("editableData").innerHTML = response;

//            // RIGHT side cells ko input me convert karo
//            document
//                .querySelectorAll("#editableData tr")
//                .forEach(tr => {

//                    tr.querySelectorAll("td").forEach((td, index) => {

//                        // index 0 = serial number column
//                        if (index === 0) return;

//                        // verify button wali cell skip
//                        if (td.querySelector("button")) return;

//                        const text = td.innerText.trim();
//                        if (!text) return;

//                        td.innerHTML =
//                            `<input class="form-control edit-field" data-col="${index}" value="${text}">`;
//                    });

//                });


//            const modal = new bootstrap.Modal(
//                document.getElementById("verifyModalvetting")
//            );
//            modal.show();
//        }
//    );
//});
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".verify-btn-vetting");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-gauravid");
    const DistrictId = btn.getAttribute("data-districtid");
    const SubQuestionMasterId = btn.getAttribute("data-subquestionmasterid");
    const QuestionMasterId = btn.getAttribute("data-questionmasterid");
    //value set for popup for save value for same parameters
    document.getElementById("mRawId").value = rawid;
    document.getElementById("mGauravId").value = gauravid;
    document.getElementById("mDistrictId").value = DistrictId;
    document.getElementById("mSubQuestionMasterId").value = SubQuestionMasterId;
    document.getElementById("mQuestionMasterId").value = QuestionMasterId;

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

            const tempDiv = document.createElement("div");
            tempDiv.innerHTML = response;

            const row = tempDiv.querySelector("tbody tr");

            const activity = row.children[1].innerText;
            const activityName = row.children[2].innerText;
            const budget = row.children[3].innerText;

            document.getElementById("readonlyData").innerHTML = `
                <div class="mb-2"><strong>गतिविधि :</strong> ${activity}</div>
                <div class="mb-2"><strong>गतिविधि नाम :</strong> ${activityName}</div>
                <div class="mb-2"><strong>बजट :</strong> ${budget}</div>
            `;

            document.getElementById("editableData").innerHTML = `
                <div class="mb-2">
                    <strong>गतिविधि :</strong> ${activity}
                      <input type="hidden" id="editActivity" value="${activity}">
                </div>
                <div class="mb-2">
                    <strong>गतिविधि नाम :</strong>${activityName}
                    <input type="hidden"  id="editActivityName" value="${activityName}">
                </div>
                <div class="mb-2">
                    <strong>बजट :</strong>
                    <input class="form-control" id="editBudget" value="${budget}">
                </div>
            `;

            const modal = new bootstrap.Modal(
                document.getElementById("verifyModalvetting")
            );
            modal.show();
        }
    );
});

document.getElementById("saveVerify")
    .addEventListener("click", function () {

        // modal stored values
        const rawid =
            document.getElementById("mRawId").value;

        const gauravid =
            document.getElementById("mGauravId").value;

        const districtId =
            document.getElementById("mDistrictId").value;

        const subQuestionId =
            document.getElementById("mSubQuestionMasterId").value;

        const questionId =
            document.getElementById("mQuestionMasterId").value;

        // edited values

        const activity =document.getElementById("editActivity").value;
        const activityName = document.getElementById("editActivityName").value;

        const budget =document.getElementById("editBudget").value;

        ajax.doPostAjax(
            "/Budget/SaveVettingData",
            {
                RowId: rawid,
                GauravId: gauravid,
                DistrictId: districtId,
                SubQuestionMasterId: subQuestionId,
                QuestionMasterId: questionId,
                Activity: activity,
                ActivityName: activityName,
                Budget: budget,
                Activity: activity,
                ActivityName: activityName,
                Budget: budget
            },
            function (res) {
               
                if (res.status) {
                    toast.showToast('success', res.message, 'success');

                    bootstrap.Modal.getInstance(
                        document.getElementById("verifyModalvetting")
                    ).hide();

                    // 2 second baad reload
                    setTimeout(function () {
                        //location.reload();
                        loadPendingList(rawid,
                            gauravid,
                            districtId,
                            subQuestionId,
                            questionId);
                    }, 2000);

                } else {
                    toast.showToast('error', res.message, 'error');
                }

              
            }
        );
    });


function bindReadonly(data) {
    document.getElementById("readonlyData").innerHTML = `
        <div><strong>गतिविधि :</strong> ${data.activity}</div>
        <div><strong>गतिविधि नाम :</strong> ${data.activityName}</div>
        <div><strong>बजट :</strong> ${data.budget}</div>
    `;
}

function bindEditable(data) {
    document.getElementById("editableData").innerHTML = `
        <div><strong>गतिविधि :</strong> ${data.activity}</div>
        <div><strong>गतिविधि नाम :</strong>
            <input class="form-control" value="${data.activityName}">
        </div>
        <div><strong>बजट :</strong>
            <input class="form-control" value="${data.budget}">
        </div>
    `;
}

function loadPendingList(rawid, gauravid, districtId,
    subQuestionId, questionId) {

    ajax.doGetAjaxVetting(
        "/Budget/GetPendingVettingList",
        {
            RowId: rawid,
            GauravId: gauravid,
            DistrictId: districtId,
            SubQuestionMasterId: subQuestionId,
            QuestionMasterId: questionId
        },
        function (response) {

            document.getElementById("pendingListContainer")
                .innerHTML = response;

            //new bootstrap.Modal(
            //    document.getElementById("verifyModalvetting")
            //).show();
        }
    );
}

document.addEventListener("click", function (e) {

    const btn = e.target.closest(".edit-btn");
    if (!btn) return;

    const tr = btn.closest("tr");

    const activityName = tr.children[2].innerText.trim();
    const budget = tr.children[3].innerText.trim();

    tr.children[2].innerHTML =
        `<input class="act-name table-input" value="${activityName}">`;

    tr.children[3].innerHTML =
        `<input class="budget table-input" value="${budget}">`;
});


document.addEventListener("click", function (e) {

    const btn = e.target.closest(".cancel-btn");
    if (!btn) return;

    const tr = btn.closest("tr");

    const actInput = tr.querySelector(".act-name");
    const budgetInput = tr.querySelector(".budget");

    if (!actInput || !budgetInput) return;

    tr.children[2].innerHTML = actInput.defaultValue;
    tr.children[3].innerHTML = budgetInput.defaultValue;
});
