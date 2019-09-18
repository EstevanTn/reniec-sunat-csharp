
Consultas RENIEC y SUNAT

# RENIEC

```
var reniec = new ReniecService();
var person = await reniec.QueryAsync("01234567");
```

### Datos disponibles
```
string Dni { get; }
string Apellidos { get; }
string Nombres { get; }
string Distrito { get; }
string Provincia { get; }
string Departamento { get; }
```

# SUNAT

```
var service = new Tunaqui.Peru.Service.SunatService();
var company = await service.QueryAsync(txtRuc.Text);
```

### Datos disponibles

```
string Ruc { get; }
string RazonSocial { get; }
string NombreComercial { get; }
string Tipo { get; }
string Estado { get; set; }
string Condicion { get; set; }
string FechaInscripcion { get; }
string FechaInicioActividades { get; }
string Direccion { get; }
string Distrito { get; }
string Provincia { get; }
string Departamento { get; }
string ProfesionOficio { get; }
string SisEmisionComprobante { get; }
string SisContabilidad { get; }
string ActComercioExterior { get; }
List<string> ActividadesEconomicas { get; }
List<string> ComprobantesPagoAutImpresion { get; }
string ObligadoEmitirCPE { get; }
List<string> SisEmisionElectronica { get; }
string EmisorElectronicoDesde { get; }
List<string> ComprobantesElectronicos { get; }
string AfiliadoPLEDesde { get; }
List<string> Padrones { get; }
```

### Nota
- agregar `useLegacyV2RuntimeActivationPolicy="true"` en el `App.config` o `Web.config` de su proyecto principal.
```
<configuration>
    <startup useLegacyV2RuntimeActivationPolicy="true"> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
    </startup>
</configuration>
```

- Copiar la carpeta `Content` dentro de su proyecto

# DEMO
https://youtu.be/XwRLl8G5uGc