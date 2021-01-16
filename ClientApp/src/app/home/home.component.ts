import { AfterViewInit, Component,Inject, ViewChild, ChangeDetectorRef } from '@angular/core';
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
  public sensors: string[];
  selectedValue: string;
  dateFrom: Date;
  dateTo: Date;
  pollutions: Pollution[] = [];
  alertPollutions: AlertPollution[] = [];
  httpClient: HttpClient;
  baseUrl: string;
  dataSource = new MatTableDataSource<Pollution>(this.pollutions);
  dataSource2 = new MatTableDataSource<Pollution>(this.alertPollutions);
  displayedColumns:  string[] = ['id', 'data', 'time', 'alr'];
  displayedColumns2:  string[] = ['id', 'data', 'time', 'about', 'alr'];
  username: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  //@ViewChild(MatPaginator, {static: false}) paginator2: MatPaginator;

  constructor(private route: ActivatedRoute, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    // http.get<string[]>(baseUrl + 'pollution/getsensors').subscribe(result => {
    //   this.sensors = result;
    // }, error => console.error(error));
    // this.httpClient = http;
    // this.baseUrl = baseUrl;
    // http.get<AlertPollution[]>(baseUrl + 'alertpollution').subscribe(result => {
    //   this.dataSource2 = new MatTableDataSource<AlertPollution>(result);
    // }, error => console.error(error));
    this.route.queryParams.subscribe(params => {
      this.username = params['username'];
  });
   if(localStorage.getItem("username")==='undefined')
      localStorage.setItem("username", this.username);
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    //this.dataSource2.paginator = this.paginator;
  }

  selected(event) {
    this.selectedValue = event.value;
  }
  public showData() {
    this.httpClient.get<Pollution[]>(this.baseUrl + 'pollution/getbyparams?sensorid='+this.selectedValue+'&from='+this.dateFrom.toDateString()+'&to='+this.dateTo.toDateString()).subscribe(result => {
      this.dataSource = new MatTableDataSource<Pollution>(result);
      this.ngAfterViewInit();
    }, error => console.error(error));
    
  }

  public createAlertPollution(pollution, event){
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.post(this.baseUrl + 'alertpollution', JSON.stringify(pollution), options)
     .subscribe((s) => {
      console.log(s);
      location.reload();
    });
  }

  public updateAlertPollution(alertPollution, event){
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    })
    };
    
   this.httpClient.put(this.baseUrl + 'alertpollution', JSON.stringify(alertPollution), options)
     .subscribe((s) => {
      console.log(s);
      location.reload();
    });
  }
}


interface Pollution {
  sensorId: string;
  sensorData: number;
  collectTime: Date;
}

interface AlertPollution {
  sensorId: string;
  sensorData: number;
  collectTime: Date;
  about: string;
}