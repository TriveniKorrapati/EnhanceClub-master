$(document).ready(function () {
    //if gender is other then show other text box
    if ($("#PatientProfileSex").val() == 'o') {
        // show other text box
        $("#gender-other-text").removeClass("d-none");
        $("#PatientProfileSexOther").attr("required", true);
    }
    else {
        $("#gender-other-text").addClass("d-none");
        $("#PatientProfileSexOther").removeAttr("required");
    }

    $("#PatientProfileSex").on('change', function () {
        if ($("#PatientProfileSex").val() == 'o') {
            // show other text box
            $("#gender-other-text").removeClass("d-none");
            $("#PatientProfileSexOther").attr("required", true);
        }
        else {
            $("#gender-other-text").addClass("d-none");
            $("#PatientProfileSexOther").removeAttr("required");
        }
    })

    $(".btn-default").on('click', function (event) {
        let firstInvalidField = getFirstInvalidRequireField();
        highlightRequiredFields();

        // Firefox will focus on a missing required field at the top of the page, hiding it behind the our top menu
        // adding a scoll to top - 125 to ensure the first required field is visible by the user on all browsers
        if (firstInvalidField) {
            event.preventDefault();
            $('html, body').animate({ scrollTop: $(firstInvalidField).offset().top - 125 }, 'fast');
        }

        if (checkMinimumAge()) {
            //if ($("#checkedValue").val() == "")
            //{
            //    alert('select any (yes/no) if you need Additional Counselling ')
            //    return false;
            //}
            //else {

            //    if ($("#checkedValue").val().toLowerCase() == "true") {
            //        var checked = $("input[type=checkbox]:checked").length;

            //        if (!checked) {
            //            alert("You must select at least one best time for consulting.");
            //            return false;
            //        }

            //    }
            //    return true;
            //}
        }
        else {
            return false;
        }
        checkedSocialHistory = $("input[type=checkbox]:checked").length;

        if (!checkedSocialHistory) {
            alert("You must check at least one Social History.");
            return false;
        }
    });

    $("#PatientProfileBirthDate").blur(function () {
        checkMinimumAge();
    });

    //  if none is selected for social history then unselect all
    $('input.social-history').on('change', function () {
        if ($(this).closest('.custom-check').find('.social-history-name').val() == 'None') {
            $('input.social-history').not(this).prop('checked', false);
        }
        else {
            $('.noneCheckBox').prop('checked', false);
        }
    });

    // $("#checkedValue").val($("input[type='radio'][name='PatientProfileConsultationConsent']").val());
    if ($("input[type='radio'][name='PatientProfileConsultationConsent']").val() == "True") {
        $("#PatientProfileConsultationConsentYes").prop("checked", true)
        $("#PatientProfileConsultationConsentNo").prop("checked", false)
        $("#consultationhours").show();
    }
    else {
        $("#PatientProfileConsultationConsentNo").prop("checked", true)
        $("#PatientProfileConsultationConsentYes").prop("checked", false)
        $("#consultationhours").hide();
    }

    $('[required]').on('blur', function () {
        if ($(this).val() !== '') {
            $(this).css('border-color', '');
        }
    })
});

function checkMinimumAge() {
    var today = new Date();
    var PatientProfileBirthDate = new Date($("#PatientProfileBirthDate").val());
    var age = Math.floor((today - PatientProfileBirthDate) / (365.25 * 24 * 60 * 60 * 1000));
    if (age < 18) {
        alert('You must be at least 18 years old!')
        return false;
    }
    else
        return true;
}

function highlightRequiredFields() {
    $('[required]').each(function () {
        if ($(this).val() === '') {
            $(this).css('border-color', 'red');
        }
    });
}

function getFirstInvalidRequireField() {
    let invalidField = null;
    $('[required]').each(function () {
        if ($(this).val() === '') {
            invalidField = $(this);
            return false;
        }
    });

    return invalidField;
}

function SetConsentValue(val) {
    $("#consentChanged").val(true);
    $("#checkedValue").val(val);
    if (val) {
        $("#consultationhours").show();
    }
    else { $("#consultationhours").hide(); }
}
