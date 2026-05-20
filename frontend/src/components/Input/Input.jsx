import { mensagemErroCampo, rotuloCampo } from '../styles/field.styles'
import { InputBase } from './InputBase'

export function Input({ rotulo, erro, id, className, ...props }) {
  const campoId = id ?? props.name

  if (!rotulo) {
    return (
      <InputBase id={campoId} erro={erro} className={className} {...props} />
    )
  }

  const erroId = campoId ? `${campoId}-erro` : undefined

  return (
    <div className="relative">
      <div className="flex flex-col gap-1.5">
        <label htmlFor={campoId} className={rotuloCampo}>
          {rotulo}
        </label>
        <InputBase
          id={campoId}
          erro={erro}
          className={className}
          aria-describedby={erro ? erroId : undefined}
          {...props}
        />
      </div>
      {erro && (
        <p id={erroId} role="alert" className={mensagemErroCampo}>
          {erro}
        </p>
      )}
    </div>
  )
}
