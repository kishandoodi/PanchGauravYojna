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
    loadPendingList(0, garauvId, districtId, 0, 0)
}
function onsavegetpendinglist(gauravid, districtId) {

    //var garauvId = document.getElementById("GarauvId").value;
    //var districtId = document.getElementById("DistrictId").value;
    if (!gauravid) return;
    if (!districtId) return;

    ajax.doPostAjaxHtml(
        "/Budget/GetVettingList",
        { garauvId: gauravid, districtId: districtId },
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
                    onsavegetpendinglist(gauravid, districtId);
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

    //const activityName = tr.children[2].innerText.trim();
    const budget = tr.children[4].innerText.trim();

    //tr.children[2].innerHTML =
    //`<input class="act-name table-input" value="${activityName}">`;

    tr.children[4].innerHTML =
        `<input class="budget table-input" value="${budget}">`;
    // Edit → Update
    // buttons toggle
    btn.classList.add("d-none");
    tr.querySelector(".update-btn")
        .classList.remove("d-none");
});
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".cancel-btn");
    if (!btn) return;

    const tr = btn.closest("tr");

    //const actInput = tr.querySelector(".act-name");
    const budgetInput = tr.querySelector(".budget");

    //if (!actInput || !budgetInput) return;
    if (!budgetInput) return;
    // original values restore
    //tr.children[2].innerHTML = actInput.defaultValue;
    tr.children[4].innerHTML = budgetInput.defaultValue;

    // buttons toggle
    tr.querySelector(".update-btn")
        .classList.add("d-none");

    tr.querySelector(".edit-btn")
        .classList.remove("d-none");

    //btn.classList.add("d-none"); // cancel hide
});
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".delete-btn");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-GauravId");
    const districtId = btn.getAttribute("data-DistrictId");
    const subQuestionId = btn.getAttribute("data-SubQuestionMasterId");
    const questionId = btn.getAttribute("data-QuestionMasterId");


    ajax.doPostAjax(
        "/Budget/DeleteVettedList",
        {
            RawId: rawid,
            garauvId: gauravid,
            DistrictId: districtId,
            SubQuestionMasterId: subQuestionId,
            QuestionMasterId: questionId
        },
        function (res) {
            if (res.status) {
                onsavegetpendinglist(gauravid, districtId);

                loadPendingList(rawid,gauravid,districtId,subQuestionId,questionId);
                
                toast.showToast('success', res.message, 'success');
            } else {
                toast.showToast('error', res.message, 'error');
            }
            
        }
    );
});
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".update-btn");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-GauravId");
    const districtId = btn.getAttribute("data-DistrictId");
    const subQuestionId = btn.getAttribute("data-SubQuestionMasterId");
    const questionId = btn.getAttribute("data-QuestionMasterId");
    //
    const tr = btn.closest("tr");

    //const activityName =
        //tr.querySelector(".act-name").value;

    const budget =
        tr.querySelector(".budget").value;

    //tr.children[2].innerText = activityName;
   // tr.children[3].innerText = budget;

    ajax.doPostAjax(
        "/Budget/UpdateVettedList",
        {
            RowId: rawid,
            GauravId: gauravid,
            DistrictId: districtId,
            SubQuestionMasterId: subQuestionId,
            QuestionMasterId: questionId,
            Budget: budget
        },
        function (res) {
            if (res.status) {
                onsavegetpendinglist(gauravid, districtId);

                loadPendingList(rawid, gauravid, districtId, subQuestionId, questionId);

                toast.showToast('success', res.message, 'success');
            } else {
                toast.showToast('error', res.message, 'error');
            }

        }
    );
});
document.addEventListener("click", function (e) {

    const btn = e.target.closest("#openVettedPopup");
    if (!btn) return;

    //const gauravId = $("#GauravId").val();
    const gauravId = 1;

    ajax.doPostAjaxHtml(
        "/Budget/VettedQuestions",
        { GauravId: gauravId },
        function (response) {

            $("#vettedModalContainer").html(response);

            let modal = new bootstrap.Modal(
                document.getElementById("vettedModal")
            );
            modal.show();

            buildVettedForm(); // JS function
        }
    );

});

function buildVettedForm() {

    let container = $("#dynamicFormContainervetted");
    container.empty();

    questions.forEach(q => {
        container.append(
            `<div class="mb-2">
                <label>${q.questionName}</label>
                <input class="form-control" value="${q.answer ?? ''}">
            </div>`
        );
    });
}



function loadDynamicForm() {
    let container = document.getElementById("dynamicFormContainervetted");
    container.innerHTML = "";
    //alert(container);
    questions.forEach(q => {
        let card = `
            <div class="p-3 mb-3 shadow-sm">
                <h5 class="fw-bold">${q.displayNumber}. ${q.questionText}</h5>      
                ${renderSubQuestionsVetted(q)}
            </div>
        `;
         
        container.innerHTML += card;
    });
}
function buildQuestionHtml(q) {
    let html = `<div class="row">`;
    q.subQuestions.forEach(sub => {

        let name = `Q_${q.questionMasterId}_${sub.subQuestionMasterId}`;

        // dropdown
        if (sub.fieldtype === "DropDown") {
            let opts = "";
            opts = `<option value="">-- Select --</option>`;
            activityList.forEach(o => {
                opts += `<option value="${o.value}">${o.text}</option>`;
            });

            html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <select name="${name}" class="form-select">${opts}</select>
                </div>
            `;
        }
        // textarea
        else if (sub.fieldtype === "TextArea") {
            html += `
                <div class="col-md-12 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <textarea class="form-control alphaspace" name="${name}" rows="2" placeholder="अपना उत्तर यहाँ लिखें..."></textarea>
                </div>
            `;
        }
        else if (sub.fieldtype === "DateTime") {

            let id = `DT_${q.questionMasterId}_${sub.subQuestionMasterId}`;

            html += `
        <div class="col-md-4 p-2">
            <label class="fw-bold">${sub.questionText}</label>
            <input 
                type="text"
                class="form-control datepicker"
                id="${id}"
                name="${name}"
                placeholder="dd/mm/yyyy"
                autocomplete="off"
            />
        </div>
    `;
        }

        else {
            if (sub.questionText == "नोडल विभाग का व्यय") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="NodalAmount" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
            else if (sub.questionText == "MPLAD, MLALAD से व्यय") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="MPLADAmount" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
            else if (sub.questionText == "CSR मद से व्यय") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="CSRAmount" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
            else if (sub.questionText == "अन्य मद द्वारा  व्यय") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="OtherAmount" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
            else if (sub.questionText == "पंच-गौरव से बजट आवश्यकता") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="PanchGauravAmount" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." readonly />
                </div>
            `;
            }
            else if (sub.questionText == "कूल प्रस्तावित व्यय") {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control numberonly" id="TotalProposed" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
            else {
                html += `
                <div class="col-md-4 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <input class="form-control alphaspace" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
        }
    });
    html += `</div> <button type="button" id="btnSaveStep2" class="btn btn-success mt-3">
        Add
    </button>`;
    return html;
}

