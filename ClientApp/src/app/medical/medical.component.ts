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
  public users: Object[] = [];
  dataSource = new MatTableDataSource<Object>(this.users);
  displayedColumns:  string[] = [];
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
  }

  ngAfterViewInit(): void {
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