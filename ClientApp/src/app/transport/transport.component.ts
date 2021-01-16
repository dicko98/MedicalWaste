import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatPaginator, MatTableDataSource, ShowOnDirtyErrorStateMatcher } from '@angular/material';

@Component({
  selector: 'app-transport',
  templateUrl: './transport.component.html'
})
export class TransportComponent implements AfterViewInit{
  public pollutions: Object[] = [];
  dataSource = new MatTableDataSource<Object>(this.pollutions);
  displayedColumns:  string[] = ['id', 'data', 'time', 'deletion'];
  httpClient: HttpClient;
  baseUrl: string;
  public sensors: string[];
  selectedValue: string;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  

  constructor(http: HttpClient) {
    http.get<Object>('https://192.168.2.148:45455/' + 'Package/GetAllPackages').subscribe(result => {
      console.log(result);  
    this.dataSource = new MatTableDataSource<Object>();
      this.ngAfterViewInit();
      
    }, error => console.error(error));
    this.httpClient = http;
    
  }

  selected(event) {
    this.selectedValue = event.value;
  }
  

  deleteData(pollution, event){
    const options = {
      headers: new HttpHeaders({
      'Content-Type': 'application/json',
    }), body: pollution
    };

   this.httpClient.delete(this.baseUrl + 'pollution/deletebykey', options)
     .subscribe((s) => {
      console.log(s);
      location.reload();
    });
  }

  deleteSensorData(){
    this.httpClient.delete(this.baseUrl + 'pollution/' + this.selectedValue)
    .subscribe((s) => {
     console.log(s);
     location.reload();
   });
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }
}

interface Pollution {
  sensorId: string;
  sensorData: number;
  collectTime: Date;
}
