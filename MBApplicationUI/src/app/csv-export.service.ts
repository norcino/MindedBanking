import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CsvExportService {

  constructor() { }

  // Export to CSV
  // ref.: https://medium.com/mycoding/export-json-to-csv-file-in-angular-d1b674ec79ed
  convertToCSV(transactions: any[], fieldsToBeExported: string[], headers: string[]) {
    let array = typeof transactions != 'object' ? JSON.parse(transactions) : transactions;
    let str = '';

    let row = '_,';
    for (let index in headers) {
      row += headers[index] + ',';
    }
    row = row.slice(0, -1);
    str += row + '\r\n';

    for (let i = 0; i < array.length; i++) {
      let line = (i+1)+'';
      for (let index in fieldsToBeExported) {
        let head = fieldsToBeExported[index];
        console.log(array[i]);

        if(head.includes('.'))
          line += ',' + this.handleNestedProperty(array[i], head);
        else
          line += ',' + array[i][head];
      }
      str += line + '\r\n';
    }
    return str;
  }

  handleNestedProperty(object: any, property: string) {
    return this.getObjectPropertyRecursively(object, property.split('.'));
  }

  getObjectPropertyRecursively(object: any, tokens: string[]): string {
    if(tokens.length > 1) {
      return this.getObjectPropertyRecursively(object[tokens[0]], tokens.slice(1));
    }
    return object[tokens[0]];
  }

  downloadFile(transactions: any[], fieldsToBeExported: string[], headers: string[], filename = 'data') {
    let csvData = this.convertToCSV(transactions, fieldsToBeExported, headers);
    let blob = new Blob(['\ufeff' + csvData], { type: 'text/csv;charset=utf-8;' });
    let dwldLink = document.createElement("a");
    let url = URL.createObjectURL(blob);
    let isSafariBrowser = navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1;

    if (isSafariBrowser) {  //if Safari open in new window to save file with random filename.
      dwldLink.setAttribute("target", "_blank");
    }

    dwldLink.setAttribute("href", url);
    dwldLink.setAttribute("download", filename + ".csv");
    dwldLink.style.visibility = "hidden";
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
  }
}
