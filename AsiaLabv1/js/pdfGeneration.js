function generateRecipt(details) {
    var selectedTests = details.PatientTests;
    var totalCharges = 0;
    var reportingDate = new Date();
    reportingDate.setDate(reportingDate.getDate() + 1);
    var date = new Date();
    var tests = [
            [{ text: 'Tests', style: 'tableHeader', alignment: 'left' },
            { text: 'Amount', style: 'tableHeader', alignment: 'right' }]
    ];
    var reportingDates = [
          [{ text: 'Reporting Date', style: 'header', alignment: 'left', decoration: 'underline' }, { text: 'Description', style: 'header', alignment: 'right', decoration: 'underline' }]
    ];

    for (var i = 0; i < selectedTests.length; i++) {
        var row = [{
            text: selectedTests[i].TestSubcategoryName, style: 'subHeader', alignment: 'left'
        }, { text: selectedTests[i].Rate+"", style: 'subHeader', alignment: 'right' }];
        var reportingDateRow = [{ text: reportingDate.toISOString(), style: 'header', alignment: 'left' }, { text: selectedTests[i].TestSubcategoryName, style: 'header', alignment: 'right' }];
        
        reportingDates.push(reportingDateRow);
        tests.push(row);
        totalCharges = totalCharges + selectedTests[i].Rate;
    }
    var netAmount = totalCharges - details.PatientDetails.Discount;
    tests.push([{ text: 'Total Charges', style: 'totalChargeStyle', alignment: 'left' }, { text: ''+totalCharges, style: 'total', alignment: 'right' }]);

    var docDefinition = {
        content: [
               { text: 'Asia Lab Diagnostic Center', style: "HeaderText" },
               { text: details.Branch.Name, style: "address1" },
               { text: details.Branch.Address, fontSize: 9 },
               { text: details.Branch.Contact, style: "telephone" },
                { text: "_______________________________________________________", style: "underlineStyle" },
               { text: 'Recipt no. ' + details.PatientDetails.Id, fontSize: 9 },
               { text: 'Name: ' + details.PatientDetails.Name, fontSize: 9 },
               { text: 'Gender: ' + details.PatientDetails.GenderDesc, fontSize: 9 },
               { text: 'Phone: ' + details.PatientDetails.PhoneNumber, fontSize: 9 },
               { text: 'e-Reports: www.asialabs.com/LabReports', fontSize: 9, margin: [30, 10, 0, 0] },
               { text: "_______________________________________________________", style: "underlineStyle" },
                { text: 'Charges:', fontSize: 9 },
                  {
                      style: 'TableStyle',
                      table: {
                          widths: [150, 50],
                          headerRows: 1,
                          body: tests
                      }, layout: 'noBorders'
                  },
                { text: "_______________________________________________________", style: "underlineStyle" },
                { text: 'Payment Recivable:', style: "underlineStyle" },
                 {
                     style: 'TableStyle',
                     table: {
                         widths: [150, 50],
                         headerRows: 1,
                         body: [
                         [{ text: 'Net Amount', style: 'header', alignment: 'left' }, { text: netAmount + ' Rs', style: 'header', alignment: 'right' }],
                         [{ text: 'Paid Amount', style: 'header', alignment: 'left' }, { text: details.PatientDetails.PaidAmount + ' Rs', style: 'header', alignment: 'right', decoration: 'underline' }],
                         [{ text: 'Net Balance Due', style: 'header', alignment: 'left' }, { text: (netAmount - details.PatientDetails.PaidAmount)+' Rs', style: 'header', alignment: 'right' }]
                         ]
                     }, layout: 'noBorders'
                 },
                 { text: "_______________________________________________________", style: "underlineStyle" },
                 { text: 'Reporting Schedule:', style: 'underlineStyle' },
                  {
                      style: 'TableStyle',
                      table: {
                          widths: [80, 130],
                          headerRows: 1,
                          body: reportingDates
                      }, layout: 'noBorders'
                  }, { text: "_______________________________________________________", style: "underlineStyle", margin: [0, 0, 0, 10] },
                 {
                     columns: [
                     {
                         width: 210,
                         fontSize: 8,
                         alignment: 'justify',
                         margin: [0, 0, 0, 10],
                         text: 'Please collect your reports between 7:30PM to 8:30 PM on on reporting day. Asia lab will not be responsible for report not collected within one month after the reporting date.'
                     }
                     ]
                 },
                 {
                     columns: [
                     {
                         width: 80,
                         fontSize: 7,
                         alignment: 'left',
                         text: 'Working Hours'
                     },
                     {
                         width: 130,
                         fontSize: 7,
                         alignment: 'right',
                         text: 'Mon to Sat  8:00AM to 12:00AM Midnight'
                     }
                     ],
                     margin: [0, 0, 0, 10]
                 },
                  { text: "_______________________________________________________", style: "underlineStyle", margin: [0, 0, 0, 10] },
                  {
                      columns: [
                      {
                          width: 80,
                          fontSize: 7,
                          alignment: 'left',
                          text: 'PrintedOn'
                      },
                      {
                          width: 130,
                          fontSize: 7,
                          alignment: 'right',
                          text: date.toISOString()
                      }
                      ],
                      margin: [0, 0, 0, 2]
                  },
                  {
                      columns: [
                      {
                          width: 80,
                          fontSize: 7,
                          alignment: 'left',
                          text: 'Printed By:'
                      },
                      {
                          width: 130,
                          fontSize: 7,
                          alignment: 'right',
                          text: details.LogedInUser
                      }
                      ]
                  },
                  { text: "_______________________________________________________", style: "underlineStyle" },
                 { text: 'Developed By: Upper Bound Technologies', style: "footerStyle" }
        ], styles: {
            footerStyle: {
                fontSize: 7,
                margin: [30, 0, 0, 0]
            },
            header: {
                fontSize: 9
            },
            subHeader: {
                fontSize: 11,
                bold: true,
                margin: [0, 0, 0, 0]
            },
            TableStyle: {
                width: 100,
                margin: [0, 5, 0, 10]
            },
            tableHeader: {
                fontSize: 10,
                margin: [0, 0, 0, 10],
                color: 'black',
                decoration: 'underline'
            },
            totalChargeStyle: {
                fontSize: 10,
                bold: true,
                margin: [0, 10, 0, 0],
                color: 'black'
            },
            HeaderText: {
                fontSize: 16,
                bold: true,
                margin: [14, -20, 0, 4],
                decoration: 'underline'
            },
            address1: {
                fontSize: 9,
                margin: [60, 0, 0, 0]
            },
            telephone: {
                fontSize: 9,
                margin: [73, 3, 0, 0]
            },
            underlineStyle:
            {
                fontSize: 9,
                bold: true
            }
        }
    };
    openPdf(docDefinition);
}

function print() {

}

function openPdf(docDefinition) {
    pdfMake.createPdf(docDefinition).open();
}