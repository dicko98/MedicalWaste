import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator, MatTableDataSource, ShowOnDirtyErrorStateMatcher } from '@angular/material';
@Component({
  selector: 'app-landfill',
  templateUrl: './landfill.component.html'
})
export class LandfillComponent implements AfterViewInit {
  httpClient: HttpClient;
  currentUser: ApplicationUser = new ApplicationUser();
  weight: string;
  barcode: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;


  constructor(http: HttpClient) {
    http.get<ApplicationUser>('https://192.168.2.148:45455/' + 'user/getlandfilluser?username=' + localStorage.getItem("username")).subscribe(result => {
      console.log(result);  
    this.currentUser = result;
    //this.ngAfterViewInit();    
    }, error => console.error(error));
   this.httpClient = http;
   this.ngAfterViewInit();
 }

  ngAfterViewInit(): void {
    //this.dataSource.paginator = this.paginator;
  }
  
  public pickUp() {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };

    this.httpClient.put('https://192.168.2.148:45455/' + 'user/storepackage?barcode='+this.barcode+'&weight='+this.weight+'&username='+ localStorage.getItem("username") + '', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste registrovali skladistenje medicinskog otpad - paket!");
      location.reload();
    }, error => alert(error));
    
  }

}

class ApplicationUser {
  firstname: string;
  lastname:string;
  username: string;
  orgname: string;
  orglocation: string;
}