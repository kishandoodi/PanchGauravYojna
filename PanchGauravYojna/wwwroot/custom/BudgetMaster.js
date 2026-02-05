document.addEventListener("DOMContentLoaded", function () {
    bindGarauv();
    bindDistrict();
    document.getElementById("DistrictId")
        .addEventListener("change", onDistrictChange);
});
function onDistrictChange() {

    var garauvId = document.getElementById("GarauvId").value;
    var districtId = document.getElementById("DistrictId").value;

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