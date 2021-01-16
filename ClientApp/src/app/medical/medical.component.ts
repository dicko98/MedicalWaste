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
  currentUser: ApplicationUser = null;
  weight: string;
  name: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;


  constructor(http: HttpClient) {
    // http.get<ApplicationUser>('https://192.168.2.148:45455/' + 'user/getmedicaluser/' + localStorage.getItem("currentUser")).subscribe(result => {
    //   console.log(result);  
    // this.currentUser = result;
    // this.ngAfterViewInit();    
    // }, error => console.error(error));
    this.httpClient = http;
    this.currentUser = new ApplicationUser();
    this.currentUser.firstname = "Milan";
    this.currentUser.lastname = "Stojanovic";
    this.currentUser.orgname = "Klinicki centar";
    this.currentUser.orglocation = "Nis";
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
    const url_ = 'api/product/findProduct';
    const params = new URLSearchParams();
    params.set('name', this.name);
    params.set('weight', this.weight);
    params.set('username', localStorage.getItem("username"));
    alert(this.name );

   this.httpClient.post('https://192.168.2.148:45455/' + 'package/createpackage?name='+this.name+'&weight='+this.weight+'&username='+ localStorage.getItem("username") + '', options)
     .subscribe((s) => {
      console.log(s);
      location.reload();
    });
    
  }
}

class ApplicationUser {
  firstname: string;
  lastname:string;
  username: string;
  orgname: string;
  orglocation: string;
}