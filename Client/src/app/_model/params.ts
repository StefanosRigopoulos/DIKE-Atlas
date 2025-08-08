export class EmployeeParams {
    rank: string = "";
    speciality: string = "";
    firstName: string = "";
    lastName: string = "";
    unit: string = "";
    office: string = "";
    pageNumber = 1;
    pageSize = 12;
    orderBy = 'lastname';
}

export class PCParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    domain: string = "";
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'brand';
}

export class CPUParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    cores: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class MOBOParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    dramSlots: string = "";
    chipsetModel: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class RAMParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    type: string = "";
    size: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class GPUParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    memory: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class PSUParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class StorageParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    type: string = "";
    interface: string = "";
    capacity: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class NetworkCardParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}

export class MonitorParams {
    barcode: string = "";
    brand: string = "";
    model: string = "";
    resolution: string = "";
    inches: string = "";
    pageNumber = 1;
    pageSize = 13;
    orderBy = 'brand';
}