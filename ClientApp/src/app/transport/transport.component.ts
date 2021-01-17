import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator, MatTableDataSource, ShowOnDirtyErrorStateMatcher } from '@angular/material';

@Component({
  selector: 'app-transport',
  templateUrl: './transport.component.html'
})
export class TransportComponent implements AfterViewInit{
  public landfills: LandfillOrganization[] = [];
  dataSource = new MatTableDataSource<LandfillOrganization>(this.landfills);
  displayedColumns:  string[] = ['deponija', 'lokacija'];
  httpClient: HttpClient;
  currentUser: ApplicationUser = new ApplicationUser();
  weight: string;
  barcode: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  

  constructor(http: HttpClient) {

    
     http.get<ApplicationUser>('https://192.168.2.148:45455/' + 'user/gettransportuser?username=' + localStorage.getItem("username")).subscribe(result => {
      console.log(result);  
    this.currentUser = result;
    http.get<LandfillOrganization[]>('https://192.168.2.148:45455/' + 'landfillorganization/getlandfillorganization?location=' + this.currentUser.orglocation).subscribe(result => {
      console.log(result);  
    this.landfills = result;
    this.dataSource = new MatTableDataSource<LandfillOrganization>(result);
    this.ngAfterViewInit();
    }, error => console.error(error));
     }, error => console.error(error));
    this.httpClient = http;
    
 


    

 }



  // deleteData(pollution, event){
  //   const options = {
  //     headers: new HttpHeaders({
  //     'Content-Type': 'application/json',
  //   }), body: pollution
  //   };

  //  this.httpClient.delete(this.baseUrl + 'pollution/deletebykey', options)
  //    .subscribe((s) => {
  //     console.log(s);
  //     location.reload();
  //   });
  // }
  a

  public pickUp() {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };

    this.httpClient.put('https://192.168.2.148:45455/' + 'user/pickuppackage?barcode='+this.barcode+'&weight='+this.weight+'&username='+ this.currentUser.username + '', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste registrovali pokupljanje medicinskog otpad - paket!");
      location.reload();
    }, error => alert(error));
    
  }


  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }
}

class ApplicationUser {
  firstname: string;
  lastname:string;
  username: string;
  orgname: string;
  orglocation: string;
}

class LandfillOrganization{
  guid: string;
  name: string;
  location: string;
}