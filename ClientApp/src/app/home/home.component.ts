import { AfterViewInit, Component,Inject, ViewChild, ChangeDetectorRef, ViewChildren, QueryList } from '@angular/core';
import {MaterialModule} from ".././material/material.module";
import { NgModule } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator } from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
@NgModule(
  {
    exports: [MaterialModule],
    imports: [MaterialModule]
  }
)
export class HomeComponent implements AfterViewInit {
 
  medorgs: MedicalOrganization[] = [];
  transportorgs: TransportOrganization[] = []; 
  landfillorgs: LandfillOrganization[] = [];
  users: ApplicationUser[] = [];
  httpClient: HttpClient;
  baseUrl: string;
  dataSource = new MatTableDataSource<MedicalOrganization>(this.medorgs);
  dataSource2 = new MatTableDataSource<TransportOrganization>(this.transportorgs);
  dataSource3 = new MatTableDataSource<LandfillOrganization>(this.landfillorgs);
  displayedColumns:  string[] = ['med', 'loc', 'del'];
  displayedColumns2: string[] = ['transportorg', 'location', 'del'];
  displayedColumns3:  string[] = ['landfillorg', 'location', 'del'];
  username: string;
  selectedMed: string;
  selectedMedUser: string;
  selectedTrans: string;
  selectedTransUser: string;
  selectedLandfill: string;
  selectedLandfillUser:string;
  nameMed: string;
  locMed: string;
  nameTrans: string;
  locTrans: string;
  nameLandfill:string;
  locLandfill:string;
  
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();


  constructor(private route: ActivatedRoute, http: HttpClient) {
    
    this.route.queryParams.subscribe(params => {
      this.username = params['username'];
  });
      localStorage.setItem("username", this.username);

      this.httpClient = http;
      http.get<MedicalOrganization[]>('https://192.168.2.148:45455/' + 'medicalorganization/getallmedicalorganizations').subscribe(result => {
      this.medorgs = result;
      console.log(result);  
      this.dataSource = new MatTableDataSource<MedicalOrganization>(result);
      this.ngAfterViewInit();
       }, error => console.error(error));
    
      http.get<TransportOrganization[]>('https://192.168.2.148:45455/' + 'transportcompany/getalltransportcompanies').subscribe(result => {
      this.transportorgs = result;
      console.log(result);  
      this.dataSource2 = new MatTableDataSource<TransportOrganization>(result);
      this.ngAfterViewInit();
      }, error => console.error(error));
  
      http.get<LandfillOrganization[]>('https://192.168.2.148:45455/' + 'landfillorganization/getalllandfillorganizations').subscribe(result => {
        this.landfillorgs = result;
        console.log(result);  
        this.dataSource3 = new MatTableDataSource<LandfillOrganization>(result);
        this.ngAfterViewInit();
      }, error => console.error(error));

      http.get<ApplicationUser[]>('https://192.168.2.148:45455/' + 'user/getallusers').subscribe(result => {
        this.users = result;
        console.log(result);  
        this.ngAfterViewInit();
      }, error => console.error(error));

  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator.toArray()[0];
    this.dataSource2.paginator = this.paginator.toArray()[1];
    this.dataSource3.paginator = this.paginator.toArray()[2];
  }

  connectMedUser(){
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'user/worksatmedical?username='+this.selectedMedUser+'&organizationGuid='+this.selectedMed +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste spojili zaposlenog za medicinsku organizaciju");
      location.reload();
    }, error => alert(error));
  }


  connectTransUser(){
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'user/worksattransport?username='+this.selectedTransUser+'&transportGuid='+this.selectedTrans +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste spojili zaposlenog sa transportnom kompanijom");
      location.reload();
    }, error => alert(error));
  }


  connectLandfillUser()
  {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'user/worksatlandfill?username='+this.selectedLandfillUser+'&landfillGuid='+this.selectedLandfill +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste spojili zaposlenog sa deponijom");
      location.reload();
    }, error => alert(error));
  }
  public createMedOrg()
  {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'medicalorganization/createmedicalorganization?name='+this.nameMed+'&location='+this.locMed +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste kreirali medicinsku organizaciju");
      location.reload();
    }, error => alert(error));
  }
  createTransOrg()
  {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'transportcompany/createtransportcompany?name='+this.nameTrans+'&location='+this.locTrans +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste kreirali transportnu kompaniju ");
      location.reload();
    }, error => alert(error));
  }
  createLandfillOrg()
  {
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post('https://192.168.2.148:45455/' + 'landfillorganization/createlandfillorganization?name='+this.nameLandfill+'&location='+this.locLandfill +'', options)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste kreirali deponiju");
      location.reload();
    }, error => alert(error));
  }

  deleteMedOrg(med: MedicalOrganization)
  {
    
   this.httpClient.delete('https://192.168.2.148:45455/' + 'medicalorganization/deletemedicalorganization?organizationGuid='+med.guid)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste obrisali medicinsku ustanovu");
      location.reload();
    }, error => alert(error));
  }
  deleteTransOrg(trans: TransportOrganization)
  {
    this.httpClient.delete('https://192.168.2.148:45455/' + 'medicalorganization/deletemedicalorganization?organizationGuid='+trans.guid)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste obrisali medicinsku ustanovu");
      location.reload();
    }, error => alert(error));
  }
  deleteLandfillOrg(land: LandfillOrganization)
  {
    this.httpClient.delete('https://192.168.2.148:45455/' + 'medicalorganization/deletemedicalorganization?organizationGuid='+land.guid)
     .subscribe((s) => {
      console.log(s);
      alert("Uspesno ste obrisali medicinsku ustanovu");
      location.reload();
    }, error => alert(error));
  }
 
}

interface MedicalOrganization{
  guid: string;
  name: string;
  location: string;
}

interface TransportOrganization{
  guid: string;
  name: string;
  location: string;
}

interface LandfillOrganization{
  guid: string;
  name: string;
  location: string;
}
class ApplicationUser {
  firstname: string;
  lastname:string;
  username: string;
  orgname: string;
  orglocation: string;
}