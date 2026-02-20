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
    $("#GauravIdVetted").val(garauvId);
    $("#DistrictIdVetted").val(districtId);
    if (!garauvId) return;
    if (!districtId) return;

    ajax.doPostAjaxHtml(
        "/Budget/GetPendingList",
        { garauvId: garauvId, districtId: districtId },
        function (response) {
            document.getElementById("vettingContainer").innerHTML = response;

        }
    );
    loadPendingList(0, garauvId, districtId, 0, 0)
}
//pending list based on gauravid and districtId
function onsavegetpendinglist(gauravid, districtId) {

    //var garauvId = document.getElementById("GarauvId").value;
    //var districtId = document.getElementById("DistrictId").value;
    if (!gauravid) return;
    if (!districtId) return;

    ajax.doPostAjaxHtml(
        "/Budget/GetPendingList",
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
    common.BindDropdown("/Budget/BindDistrictDropDownAll", "DistrictId", "District", 0);
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
            const total = row.children[3].innerText;
            const nodal = row.children[4].innerText;
            const MPLAD = row.children[5].innerText;
            const CSR = row.children[6].innerText;
            const other = row.children[7].innerText;
            const panchgaurav = row.children[8].innerText;
            const workplan = row.children[9].innerText;
            const date = row.children[10].innerText;

            
            document.getElementById("readonlyData").innerHTML = `
<div class="row mb-2">
    <div class="col-md-4 fw-bold">गतिविधि :</div>
    <div class="col-md-8">${activity}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">गतिविधि नाम :</div>
    <div class="col-md-8">${activityName}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">कूल प्रस्तावित व्यय :</div>
    <div class="col-md-8">${total}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">नोडल विभाग का व्यय :</div>
    <div class="col-md-8">${nodal}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">MPLAD, MLALAD से व्यय :</div>
    <div class="col-md-8">${MPLAD}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">CSR मद से व्यय :</div>
    <div class="col-md-8">${CSR}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">अन्य मद द्वारा व्यय :</div>
    <div class="col-md-8">${other}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">पंच-गौरव से बजट आवश्यकता :</div>
    <div class="col-md-8">${panchgaurav}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">कार्य योजना :</div>
    <div class="col-md-8">${workplan}</div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">समय सीमा :</div>
    <div class="col-md-8">${date}</div>
</div>
`;

    
            document.getElementById("editableData").innerHTML = `
<div class="row mb-2">
    <div class="col-md-4 fw-bold">गतिविधि :</div>
    <div class="col-md-8">
        ${activity}
        <input type="hidden" id="editActivity" value="${activity}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">गतिविधि नाम :</div>
    <div class="col-md-8">
        ${activityName}
        <input type="hidden" id="editActivityName" value="${activityName}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">कूल प्रस्तावित व्यय :</div>
    <div class="col-md-8">
        <input class="form-control" id="totalproposed" value="${total}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">नोडल विभाग का व्यय :</div>
    <div class="col-md-8">
        <input class="form-control" id="nodal" value="${nodal}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">MPLAD, MLALAD से व्यय :</div>
    <div class="col-md-8">
        <input class="form-control" id="MPLAD" value="${MPLAD}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">CSR मद से व्यय :</div>
    <div class="col-md-8">
        <input class="form-control" id="CSR" value="${CSR}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">अन्य मद द्वारा व्यय :</div>
    <div class="col-md-8">
        <input class="form-control" id="other" value="${other}">
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">पंच-गौरव से बजट आवश्यकता :</div>
    <div class="col-md-8">
        <input class="form-control" id="panchgaurav" value="${panchgaurav}" readonly>
    </div>
</div>

<div class="row mb-2">
    <div class="col-md-4 fw-bold">कार्य योजना :</div>
    <div class="col-md-8">
        <input class="form-control" id="workplan" value="${workplan}">
    </div>
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

        //const subQuestionId =
        //    document.getElementById("mSubQuestionMasterId").value;

        //const questionId =
        //    document.getElementById("mQuestionMasterId").value;

        // edited values

        const activity = document.getElementById("editActivity").value;
        const activityName = document.getElementById("editActivityName").value;
        const totalproposed = document.getElementById("totalproposed").value;
        const nodal = document.getElementById("nodal").value;
        const MPLAD = document.getElementById("MPLAD").value;
        const CSR = document.getElementById("CSR").value;
        const other = document.getElementById("other").value;
        const panchgaurav = document.getElementById("panchgaurav").value;
        const workplan = document.getElementById("workplan").value;

        ajax.doPostAjax(
            "/Budget/SaveVettingData",
            {
                RowId: rawid, GauravId: gauravid, DistrictId: districtId,
                //SubQuestionMasterId: subQuestionId,
                //QuestionMasterId: questionId,
                Activity: activity, ActivityName: activityName,
                Activity: activity, ActivityName: activityName, Budget: panchgaurav, Nodal: nodal, MPLAD: MPLAD,
                CSR: CSR, other: other, panchgaurav: panchgaurav, workplan: workplan, TotalProposed:totalproposed
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
                        loadPendingList(rawid,gauravid, districtId);
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
// vetted list
function loadPendingList(rawid, gauravid, districtId) {
    //$("#GauravIdHidden").val(gauravid);
    ajax.doGetAjaxVetting(
        "/Budget/GetPendingVettingList",
        {
            RowId: rawid,
            GauravId: gauravid,
            DistrictId: districtId
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
    const totalproposed = tr.children[3].innerText.trim();
    const nodal = tr.children[4].innerText.trim();
    const mplad = tr.children[5].innerText.trim();
    const csr = tr.children[6].innerText.trim();
    const other = tr.children[7].innerText.trim();
    const panchgaurav = tr.children[8].innerText.trim();
    const workplan = tr.children[9].innerText.trim();

    //tr.children[2].innerHTML =
    //`<input class="act-name table-input" value="${activityName}">`;

    tr.children[3].innerHTML =
        `<input class="totalproposed table-input" value="${totalproposed}">`;
    tr.children[4].innerHTML =
        `<input class="nodal table-input" value="${nodal}">`;
    tr.children[5].innerHTML =
        `<input class="mplad table-input" value="${mplad}">`;
    tr.children[6].innerHTML =
        `<input class="csr table-input" value="${csr}">`;
    tr.children[7].innerHTML =
        `<input class="other table-input" value="${other}">`;
    tr.children[8].innerHTML =
        `<input class="panchgaurav table-input" value="${panchgaurav}">`;
    tr.children[9].innerHTML =
        `<input class="workplan table-input" value="${workplan}">`;
    // Edit → Update
    // buttons toggle
    //btn.classList.add("d-none");
    //tr.querySelector(".update-btn")
    //    .classList.remove("d-none");
});
//document.addEventListener("click", function (e) {

//    const btn = e.target.closest(".cancel-btn");
//    if (!btn) return;

//    const tr = btn.closest("tr");

//    // find the input elements inserted by Edit mode
//    const totalInput = tr.querySelector(".totalproposed");
//    if (!totalInput) return; // nothing to cancel

//    const nodalInput = tr.querySelector(".nodal");
//    const mpladInput = tr.querySelector(".mplad");
//    const csrInput = tr.querySelector(".csr");
//    const otherInput = tr.querySelector(".other");
//    const panchInput = tr.querySelector(".panchgaurav");
//    const workplanInput = tr.querySelector(".workplan");

//    // restore original text from the inputs' defaultValue
//    tr.children[3].innerText = totalInput.defaultValue;
//    tr.children[4].innerText = nodalInput ? nodalInput.defaultValue : "";
//    tr.children[5].innerText = mpladInput ? mpladInput.defaultValue : "";
//    tr.children[6].innerText = csrInput ? csrInput.defaultValue : "";
//    tr.children[7].innerText = otherInput ? otherInput.defaultValue : "";
//    tr.children[8].innerText = panchInput ? panchInput.defaultValue : "";
//    tr.children[9].innerText = workplanInput ? workplanInput.defaultValue : "";

//    // buttons toggle
//    const updateBtn = tr.querySelector(".update-btn");
//    if (updateBtn) {
//        updateBtn.classList.add("d-none");
//    }

//    const editBtn = tr.querySelector(".edit-btn");
//    if (editBtn) {
//        editBtn.classList.remove("d-none");
//    }
//});
document.addEventListener("click", function (e) {

    const btn = e.target.closest(".delete-btn");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-GauravId");
    const districtId = btn.getAttribute("data-DistrictId");
    //const subQuestionId = btn.getAttribute("data-SubQuestionMasterId");
    //const questionId = btn.getAttribute("data-QuestionMasterId");


    ajax.doPostAjax(
        "/Budget/DeleteVettedList",
        {
            RawId: rawid,
            garauvId: gauravid,
            DistrictId: districtId,
            //SubQuestionMasterId: subQuestionId,
            //QuestionMasterId: questionId
        },
        function (res) {
            if (res.status) {
                onsavegetpendinglist(gauravid, districtId);

               // loadPendingList(rawid, gauravid, districtId, subQuestionId, questionId);
                loadPendingList(0, gauravid, districtId, 0, 0)
               toast.showToast('success', res.message, 'success');
            } else {
                toast.showToast('error', res.message, 'error');
            }

        }
    );
});
//document.addEventListener("click", function (e) {

//    const btn = e.target.closest(".update-btn");
//    if (!btn) return;

//    const rawid = btn.getAttribute("data-rowid");
//    const gauravid = btn.getAttribute("data-GauravId");
//    const districtId = btn.getAttribute("data-DistrictId");
//    //const subQuestionId = btn.getAttribute("data-SubQuestionMasterId");
//    //const questionId = btn.getAttribute("data-QuestionMasterId");
//    //
//    const tr = btn.closest("tr");

//    //const activityName =
//    //tr.querySelector(".act-name").value;

//    const totalproposed =tr.querySelector(".totalproposed").value;
//    const nodal = tr.querySelector(".nodal").value;
//    const mplad = tr.querySelector(".mplad").value;
//    const csr = tr.querySelector(".csr").value;
//    const other = tr.querySelector(".other").value;
//    const panchgaurav = tr.querySelector(".panchgaurav").value;
//    const workplan = tr.querySelector(".workplan").value;


//    //tr.children[2].innerText = activityName;
//    // tr.children[3].innerText = budget;

//    ajax.doPostAjax(
//        "/Budget/UpdateVettedList",
//        {
//            RowId: rawid,
//            GauravId: gauravid,
//            DistrictId: districtId,
//            //SubQuestionMasterId: subQuestionId,
//            //QuestionMasterId: questionId,
//            TotalProposed: totalproposed,
//            Nodal: nodal,
//            MPLAD: mplad,
//            CSR: csr,
//            other: other,
//            panchgaurav: panchgaurav,
//            workplan: workplan,
//        },
//        function (res) {
//            if (res.status) {
//                onsavegetpendinglist(gauravid, districtId);

//                loadPendingList(rawid, gauravid, districtId);

//                toast.showToast('success', res.message, 'success');
//            } else {
//                toast.showToast('error', res.message, 'error');
//            }

//        }
//    );
//});
document.addEventListener("click", function (e) {

    const btn = e.target.closest("#openVettedPopup");
    if (!btn) return;
    const selG = document.getElementById("GarauvId")?.value;
    const selD = document.getElementById("DistrictId")?.value;

    const invalidG = !selG || selG === "" || selG === "-1";
    const invalidD = !selD || selD === "" || selD === "-1";

    if (invalidG || invalidD) {
        // User must select both — prevent existing handler from opening modal
        if (invalidG) {
            toast.showToast('error', 'Please select Gaurav before adding a vetted entry.', 'error');
        } else {
            toast.showToast('error', 'Please select District before adding a vetted entry.', 'error');
        }

        // Stop other click handlers (including the existing #openVettedPopup handler)
        e.stopImmediatePropagation();
        e.preventDefault();
        return;
    }

    // Both selected — bind values to the button so existing logic can use data-gauravid / data-districtid
    btn.setAttribute('data-gauravid', selG);
    btn.setAttribute('data-districtid', selD);

    const gauravId = btn.getAttribute('data-gauravid') 
  
    if (!gauravId) return;

    ajax.doPostAjaxHtml(
        "/Budget/VettedQuestions",
        { GauravId: gauravId },
        function (response) {

            $("#vettedModalContainer").html(response);

            let modal = new bootstrap.Modal(
                document.getElementById("vettedModal")
            );
            modal.show();

            // if a separate manageMaster script exists use it, otherwise call local loader
            if (typeof buildVettedForm === 'function') buildVettedForm();
            if (typeof loadDynamicForm === 'function') loadDynamicForm();
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
                <h5 class="fw-bold">${q.questionText}</h5>      
                ${buildQuestionHtml(q)}
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
                    <select name="${name}" id="ActivityId" class="form-select">${opts}</select>
                </div>
            `;
        }
        // textarea
        else if (sub.fieldtype === "TextArea") {
            html += `
                <div class="col-md-12 p-2">
                    <label class="fw-bold">${sub.questionText}</label>
                    <textarea class="form-control alphaspace" id="WorkPlan" name="${name}" rows="2" placeholder="अपना उत्तर यहाँ लिखें..."></textarea>
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
              
                 id="CompletionDate"
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
                    <input class="form-control alphaspace" id="ActivityName" name="${name}" type="text" placeholder="अपना उत्तर यहाँ लिखें..." />
                </div>
            `;
            }
        }
    });
    html += `</div> <button type="button" id="savevettedquestions" class="btn btn-success mt-3">
        Add
    </button>`;
    return html;
}
function savevettedquestions() {

    var garauvId = $('#GarauvId').val();
    var districtId = $('#DistrictId').val();
   

    var model = {
        ActivityId: $('#ActivityId').val(),
        activityText: $('#ActivityId option:selected').text(),
        ActivityName: $('#ActivityName').val(),
        TotalProposed: Number($("#TotalProposed").val()) || null,
        NodalAmount: Number($("#NodalAmount").val()) || null,
        MPLADAmount: Number($("#MPLADAmount").val()) || null,
        CSRAmount: Number($("#CSRAmount").val()) || null,
        OtherAmount: Number($("#OtherAmount").val()) || null,
        PanchGauravAmount: Number($("#PanchGauravAmount").val()) || null,
        WorkPlan: $('#WorkPlan').val(),
        CompletionDate: $('#CompletionDate').val()
    };


    //$.ajax({
    //    url: `/Budget/savevettedquestions?GauravId=${garauvId}&DistrictId=${districtId}`,
    //    type: "POST",
    //    contentType: "application/json",
    //    data: JSON.stringify(model),

    //    success: function (data) {
    //        if (data.status) {

    //            var modalEL = document.getElementById('vettedModal'); // correct id
    //            var modal = bootstrap.Modal.getInstance(modalEL);

    //            if (modal) {
    //                modal.hide();
    //            }

    //            loadPendingList(0, garauvId, districtId, 0, 0)

    //            toast.showToast('success', data.message, 'success');
    //        } else {
    //            toast.showToast('error', data.message, 'error');
    //        }
    //    },

    //});

    $.ajax({
        url: `/Budget/savevettedquestions?GauravId=${garauvId}&DistrictId=${districtId}`,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(model),
        success: function (data) {

            if (data.status) {
                var modalEL = document.getElementById('vettedModal');
                var modal = bootstrap.Modal.getInstance(modalEL);
                if (modal) {
                                modal.hide();
                            }

                            loadPendingList(0, garauvId, districtId, 0, 0)
                toast.showToast('success', data.message, 'success');
            }
        },
        error: function (xhr) {

            if (xhr.responseJSON && xhr.responseJSON.errors) {

                xhr.responseJSON.errors.forEach(function (msg) {
                    toast.showToast('error', msg, 'error');
                });

            } else {
                toast.showToast('error', "Something went wrong", 'error');
            }
        }
    });

}

// NEW: Open modal edit handler for separate button (.edit-btn-vetted)
// Keeps all existing logic unchanged.
document.addEventListener("click", function (e) {
    const btn = e.target.closest(".edit-btn-vetted");
    if (!btn) return;

    const rawid = btn.getAttribute("data-rowid");
    const gauravid = btn.getAttribute("data-GauravId");
    const DistrictId = btn.getAttribute("data-DistrictId");
    const VettedByDepartment = btn.getAttribute("data-VettedByDepartment");

    // set modal hidden inputs if present
    const setIfExists = (id, value) => {
        const el = document.getElementById(id);
        if (el) el.value = value ?? "";
    };
    setIfExists("mRawId", rawid);
    setIfExists("mGauravId", gauravid);
    setIfExists("mDistrictId", DistrictId);

    // read row values (for editable panel)
    const tr = btn.closest("tr");
    if (!tr) return;

    const activityRow = tr.children[1]?.innerText.trim() ?? "";
    const activityNameRow = tr.children[2]?.innerText.trim() ?? "";
    const totalRow = tr.children[3]?.innerText.trim() ?? "";
    const nodalRow = tr.children[4]?.innerText.trim() ?? "";
    const MPLADRow = tr.children[5]?.innerText.trim() ?? "";
    const CSRRow = tr.children[6]?.innerText.trim() ?? "";
    const otherRow = tr.children[7]?.innerText.trim() ?? "";
    const panchgauravRow = tr.children[8]?.innerText.trim() ?? "";
    const workplanRow = tr.children[9]?.innerText.trim() ?? "";
    const dateRow = (tr.children.length > 10) ? tr.children[10]?.innerText.trim() ?? "" : "";

    // fetch server row for readonly panel (keeps server authoritative)
    ajax.doPostAjaxHtml(
        "/Budget/GetPendingVettingList",
        {
            RawId: rawid,
            garauvId: gauravid,
            DistrictId: DistrictId
        },
        function (response) {
            const tmp = document.createElement("div");
            tmp.innerHTML = response;
            const serverRow = tmp.querySelector("tbody tr");

            const activityServer = serverRow ? (serverRow.children[1]?.innerText ?? activityRow) : activityRow;
            const activityNameServer = serverRow ? (serverRow.children[2]?.innerText ?? activityNameRow) : activityNameRow;
            const totalServer = serverRow ? (serverRow.children[3]?.innerText ?? totalRow) : totalRow;
            const nodalServer = serverRow ? (serverRow.children[4]?.innerText ?? nodalRow) : nodalRow;
            const MPLADServer = serverRow ? (serverRow.children[5]?.innerText ?? MPLADRow) : MPLADRow;
            const CSRServer = serverRow ? (serverRow.children[6]?.innerText ?? CSRRow) : CSRRow;
            const otherServer = serverRow ? (serverRow.children[7]?.innerText ?? otherRow) : otherRow;
            const panchgauravServer = serverRow ? (serverRow.children[8]?.innerText ?? panchgauravRow) : panchgauravRow;
            const workplanServer = serverRow ? (serverRow.children[9]?.innerText ?? workplanRow) : workplanRow;
            const dateServer = serverRow && serverRow.children.length > 10 ? (serverRow.children[10]?.innerText ?? dateRow) : dateRow;

            // If VettedByDepartment == "1" show NA in readonly panel, otherwise show server values.
            const showNA = String(VettedByDepartment) === "1";
            const rActivity = showNA ? "NA" : activityServer;
            const rActivityName = showNA ? "NA" : activityNameServer;
            const rTotal = showNA ? "NA" : totalServer;
            const rNodal = showNA ? "NA" : nodalServer;
            const rMPLAD = showNA ? "NA" : MPLADServer;
            const rCSR = showNA ? "NA" : CSRServer;
            const rOther = showNA ? "NA" : otherServer;
            const rPanch = showNA ? "NA" : panchgauravServer;
            const rWorkplan = showNA ? "NA" : workplanServer;
            const rDate = showNA ? "NA" : dateServer;

            // fill readonly panel with either NA or server values
            const readonlyEl = document.getElementById("readonlyData");
            if (readonlyEl) {
                readonlyEl.innerHTML = `
<div class="row mb-2"><div class="col-md-4 fw-bold">गतिविधि :</div><div class="col-md-8">${rActivity}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">गतिविधि नाम :</div><div class="col-md-8">${rActivityName}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">कूल प्रस्तावित व्यय :</div><div class="col-md-8">${rTotal}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">नोडल विभाग का व्यय :</div><div class="col-md-8">${rNodal}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">MPLAD, MLALAD से व्यय :</div><div class="col-md-8">${rMPLAD}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">CSR मद से व्यय :</div><div class="col-md-8">${rCSR}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">अन्य मद द्वारा व्यय :</div><div class="col-md-8">${rOther}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">पंच-गौरव से बजट आवश्यकता :</div><div class="col-md-8">${rPanch}</div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">कार्य योजना :</div><div class="col-md-8">${rWorkplan}</div></div>
${rDate ? `<div class="row mb-2"><div class="col-md-4 fw-bold">समय सीमा :</div><div class="col-md-8">${rDate}</div></div>` : ''}
                `;
            }

            // fill editable panel from table row values (not server) - unchanged
            const editableEl = document.getElementById("editableData");
            if (editableEl) {
                const esc = s => (s===null||s===undefined) ? '' : String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
                editableEl.innerHTML = `
<div class="row mb-2"><div class="col-md-4 fw-bold">गतिविधि :</div><div class="col-md-8">${activityRow}<input type="hidden" id="editActivity" value="${esc(activityRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">गतिविधि नाम :</div><div class="col-md-8">${activityNameRow}<input type="hidden" id="editActivityName" value="${esc(activityNameRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">कूल प्रस्तावित व्यय :</div><div class="col-md-8"><input class="form-control" id="totalproposed" value="${esc(totalRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">नोडल विभाग का व्यय :</div><div class="col-md-8"><input class="form-control" id="nodal" value="${esc(nodalRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">MPLAD, MLALAD से व्यय :</div><div class="col-md-8"><input class="form-control" id="MPLAD" value="${esc(MPLADRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">CSR मद से व्यय :</div><div class="col-md-8"><input class="form-control" id="CSR" value="${esc(CSRRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">अन्य मद द्वारा व्यय :</div><div class="col-md-8"><input class="form-control" id="other" value="${esc(otherRow)}"></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">पन्च-गौरव से बजट आवश्यकता :</div><div class="col-md-8"><input class="form-control" id="panchgaurav" value="${esc(panchgauravRow)}" readonly></div></div>
<div class="row mb-2"><div class="col-md-4 fw-bold">कार्य योजना :</div><div class="col-md-8"><input class="form-control" id="workplan" value="${esc(workplanRow)}"></div></div>
                `;
            }

            // open the modal used by verify flow
            const modalEl = document.getElementById("verifyModalvetting");
            if (modalEl) {
                const modal = new bootstrap.Modal(modalEl);
                modal.show();
            }
        }
    );
});


