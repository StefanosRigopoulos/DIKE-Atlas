import { CPU, GPU, MOBO, Monitor, NetworkCard, PSU, RAM, StorageDevice } from "./parts";

export interface PC {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    pcName: string;
    administratorCode: string;
    biosCode: string;
    pdfReportPath: string;
    domain: string;
    ip: string;
    externalIP: string;
    subnetMask: string;
    gateway: string;
    dnS1: string;
    dnS2: string;
    employeeIDs: number[];
    cpuiDs: number[];
    moboiDs: number[];
    ramiDs: number[];
    gpuiDs: number[];
    psuiDs: number[];
    storageIDs: number[];
    networkCardIDs: number[];
    monitorIDs: number[];
}

export interface PCOnly {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    pcName: string;
    administratorCode: string;
    biosCode: string;
    pdfReportPath: string;
    domain: string;
    ip: string;
    externalIP: string;
    subnetMask: string;
    gateway: string;
    dnS1: string;
    dnS2: string;
}

export interface PCWithComponents {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    pcName: string;
    administratorCode: string;
    biosCode: string;
    pdfReportPath: string;
    domain: string;
    ip: string;
    externalIP: string;
    subnetMask: string;
    gateway: string;
    dnS1: string;
    dnS2: string;
    cpUs: CPU[];
    mobOs: MOBO[];
    raMs: RAM[];
    gpUs: GPU[];
    psUs: PSU[];
    storages: StorageDevice[];
    networkCards: NetworkCard[];
    monitors: Monitor[];
}