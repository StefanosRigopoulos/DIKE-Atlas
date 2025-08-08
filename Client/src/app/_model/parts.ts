export interface CPU {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    cores: number;
    threads: number;
    specification: string;
    package: string;
    chipset: string;
    integratedGPUModel: string;
    pcids: number[];
}

export interface MOBO {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    dramSlots: number;
    chipsetVendor: string;
    chipsetModel: string;
    biosBrand: string;
    biosVersion: string;
    onBoardGPUBrand: string;
    onBoardGPUModel: string;
    onBoardNetworkAdapterBrand: string;
    onBoardNetworkAdapterModel: string;
    pcids: number[];
}

export interface RAM {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    type: string;
    size: string;
    frequency: string;
    casLatency: string;
    pcids: number[];
}

export interface GPU {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    memory: string;
    driverVersion: string;
    pcids: number[];
}

export interface PSU {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    wattage: number;
    certification: string;
    pcids: number[];
}

export interface StorageDevice {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    type: string;
    interface: string;
    speed: string;
    capacity: string;
    pcids: number[];
}

export interface NetworkCard {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
}

export interface Monitor {
    id: number;
    barcode: string;
    serialNumber: string;
    brand: string;
    model: string;
    resolution: string;
    inches: number;
}