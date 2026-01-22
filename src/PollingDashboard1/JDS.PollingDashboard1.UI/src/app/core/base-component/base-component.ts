export abstract class BaseComponent {

  getClassName(): string {
    return this.constructor.name;
  }
}
