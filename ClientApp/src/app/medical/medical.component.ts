import { AfterViewInit, Component, Inject, NgModule, ViewChild } from '@angular/core';
import {MaterialModule} from ".././material/material.module";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator, MatTableDataSource, ShowOnDirtyErrorStateMatcher } from '@angular/material';

@Component({
  selector: 'app-medical',
  templateUrl: './medical.component.html'
})
@NgModule(
  {
    exports: [MaterialModule],
    imports: [MaterialModule]
  }
)

export class MedicalComponent implements AfterViewInit {
  public packages: Package[] = [];
  dataSource = new MatTableDataSource<Package>(this.packages);
  displayedColumns:  string[] = ['paket','tezina','vreme','pokupljenaTezina','skladistenaTezina','transport','deponija'];
  httpClient: HttpClient;
  currentUser: ApplicationUser = new ApplicationUser();
  weight: string;
  name: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;


  constructor(http: HttpClient) {

     http.get<ApplicationUser>('https://192.168.2.148:45455/' + 'user/getmedicaluser?username=' + localStorage.getItem("username")).subscribe(result => {
       console.log(result);  
     this.currentUser = result;
     //this.ngAfterViewInit();    
     }, error => console.error(error));
    this.httpClient = http;
    this.ngAfterViewInit();
    
    http.get<Package[]>('https://192.168.2.148:45455/' + 'package/getmedicaltrack?username=' + localStorage.getItem("username")).subscribe(result => {
      console.log(result);  
    this.packages = result;
    this.dataSource = new MatTableDataSource<Package>(result);
    this.ngAfterViewInit();
    }, error => console.error(error));

  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }


  public createPackage() {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'package/createpackage?name='+this.name+'&weight='+this.weight+'&username='+ this.currentUser.username + '', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste registrovali medicinski otpad - paket!");
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

class Package{
  name: string;
  weight: number;
  pickedweight: number;
  storedweight: number;
  datecreated: Date;
  landfillorganization: LandfillOrganization;
  transportcompany: TransportCompany;

}

class LandfillOrganization{
  guid: string;
  name: string;
  location: string;
}

class TransportCompany{
  guid: string;
  name: string;
  location: string;
}