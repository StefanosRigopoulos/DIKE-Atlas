export interface Employee {
    id: number;
    am: string;
    rank: string;
    speciality: string;
    firstName: string;
    lastName: string;
    unit: string;
    office: string;
    position: string;
    pcUsername: string;
    shdaedUsername: string;
    phone: string;
    mobile: string;
    email: string;
    photoPath: string;
    pciDs: number[];
}

export interface EmployeeOnly {
    id: number;
    am: string;
    rank: string;
    speciality: string;
    firstName: string;
    lastName: string;
    unit: string;
    office: string;
    position: string;
    pcUsername: string;
    shdaedUsername: string;
    phone: string;
    mobile: string;
    email: string;
    photoPath: string;
}