import { Button } from '../../../../components/Button'
import { Input } from '../../../../components/Input'
import { Select } from '../../../../components/Select'
import { TIPOS_ATIVO } from '../../constants/tiposAtivo'

export function FormularioAtivo({ formulario, errosCampos, atualizarCampo, enviando, enviarFormulario }) {
  return (
    <form noValidate onSubmit={enviarFormulario} className="grid gap-x-4 gap-y-6 sm:grid-cols-2 lg:grid-cols-5">
      <Input
        rotulo="Ticker"
        id="ticker"
        erro={errosCampos.ticker}
        placeholder="Ex: PETR4"
        value={formulario.ticker}
        onChange={atualizarCampo('ticker')}
      />

      <Input
        rotulo="Preço médio"
        id="precoMedio"
        erro={errosCampos.precoMedio}
        type="number"
        step="0.01"
        min="0"
        placeholder="0,00"
        value={formulario.precoMedio}
        onChange={atualizarCampo('precoMedio')}
      />

      <Input
        rotulo="Quantidade"
        id="quantidade"
        erro={errosCampos.quantidade}
        type="number"
        step="0.0001"
        min="0"
        placeholder="0"
        value={formulario.quantidade}
        onChange={atualizarCampo('quantidade')}
      />

      <Select rotulo="Tipo" id="tipo" erro={errosCampos.tipo} value={formulario.tipo} onChange={atualizarCampo('tipo')}>
        {TIPOS_ATIVO.map((tipo) => (
          <option key={tipo.valor} value={tipo.valor}>
            {tipo.rotulo}
          </option>
        ))}
      </Select>

      <div className="flex flex-col justify-end">
        <Button type="submit" disabled={enviando} className="w-full">
          {enviando ? 'Salvando...' : 'Adicionar'}
        </Button>
      </div>
    </form>
  )
}
